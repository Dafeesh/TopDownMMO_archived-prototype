using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using MasterServer.Database;

using SharedComponents.Global;
using SharedComponents.Server;

using Extant;
using Extant.Networking;

namespace MasterServer.Host
{
    public class ClientAcceptor : ThreadRun
    {
        private const int BACKLOG_MAX = 50;
        private const int RECEIVE_TIMEOUT = 3000;

        TcpListener listener;
        List<AuthorizingConnection> connections = new List<AuthorizingConnection>();
        Queue<AuthorizedLoginAttempt> authorizedLoginAttempts = new Queue<AuthorizedLoginAttempt>();
        object authorizedLoginAttempts_lock = new object();
        DBConnections.Accounts dbConnection = new DBConnections.Accounts();

        public ClientAcceptor(IPEndPoint localEndPoint)
            : base("ClientAcceptor")
        {
            this.listener = new TcpListener(localEndPoint);

            Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            listener.Start(BACKLOG_MAX);

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            AcceptConnections();
            HandleConnections();
        }

        protected override void Finish(bool success)
        {
            foreach (var c in connections)
            {
                c.Connection.Dispose();
            }

            listener.Stop();

            Log.Log("Finished.");
        }

        private void HandleConnections()
        {
            //Delete if true...
            connections.RemoveAll((c) =>
            {

                if (c.Connection.IsClosed)
                {
                    //Dead connection
                    c.Connection.Dispose();
                    return true;
                }
                else if (c.Timer.ElapsedMilliseconds > RECEIVE_TIMEOUT)
                {
                    //Time out connection
                    c.Connection.Dispose();
                    return true;
                }
                else
                {
                    //Check for authorization
                    c.HandleAuthorization();
                    if (c.IsAuthorized == null)
                    {
                        //Received nothing
                        return false;
                    }
                    else if (c.IsAuthorized == true)
                    {
                        //Authorized successfully
                        Log.Log("Client authorized successfully: " + c.AcctInfo.Name);
                        lock (authorizedLoginAttempts_lock)
                        {
                            authorizedLoginAttempts.Enqueue(new AuthorizedLoginAttempt(c.AcctInfo, c.Connection));
                        }
                        return true;
                    }
                    else //aka -> if (c.IsAuthorized == false)
                    {
                        //Error while authorizing
                        Log.Log("Client encountered an error while authorizing.");
                        c.Connection.Dispose();
                        return true;
                    }
                }
            });
        }

        private void AcceptConnections()
        {
            while (listener.Pending())
            {
                NetConnection c = new NetConnection(listener.AcceptTcpClient());
                c.Start();
                connections.Add(new AuthorizingConnection(c));
            }
        }

        public AuthorizedLoginAttempt GetAuthorizedLoginAttempt()
        {
            lock (authorizedLoginAttempts_lock)
            {
                if (authorizedLoginAttempts.Count > 0)
                    return authorizedLoginAttempts.Dequeue();
                else
                    return null;
            }
        }

        public class AuthorizedLoginAttempt
        {
            public readonly AccountInfo Info;
            public readonly NetConnection Connection;

            public AuthorizedLoginAttempt(AccountInfo info, NetConnection con)
            {
                this.Info = info;
                this.Connection = con;
            }
        }

        private class AuthorizingConnection : ILogging
        {
            public readonly NetConnection Connection;
            public readonly Stopwatch Timer;

            private DebugLogger _log;

            public bool? IsAuthorized
            { get; private set; }
            public AccountInfo AcctInfo = null;

            private IPacketDistributor distributor;

            public AuthorizingConnection(NetConnection c)
            {
                this.Connection = c;
                this.Timer = new Stopwatch();

                distributor = new ClientToMasterPackets.Distribution()
                {
                    out_AccountAuthorize_Attempt_m = OnReceive_AccountAuthorize_Attempt_m
                };

                Timer.Start();
                Log = new DebugLogger("AuthingCon");
            }

            public void HandleAuthorization()
            {
                bool receivedPacket = Connection.DistributePacket(distributor);

                if (receivedPacket == true && IsAuthorized == null)
                {
                    //Invalid packet
                    Connection.Dispose();
                    IsAuthorized = false;
                }
            }

            private void OnReceive_AccountAuthorize_Attempt_m(ClientToMasterPackets.AccountAuthorize_Attempt_m p)
            {
                if (p.Build == GameVersion.Build)
                {
                    using (DBConnections.Accounts dbConnection = new DBConnections.Accounts())
                    {
                        bool loginSuccess = dbConnection.Verify_AccountLogin(p.Username, p.Password);

                        if (loginSuccess)
                        {
                            Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.Success));

                            AcctInfo = dbConnection.Fetch_AccountInfo(p.Username);
                            IsAuthorized = true;
                        }
                        else
                        {
                            Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidLogin));
                            Connection.Close();
                            Log.Log("Client login attempt failed: invalid login.");
                            IsAuthorized = false;
                        }
                    }
                }
                else
                {
                    Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidBuild));
                    Connection.Close();
                    Log.Log("Client login attempt failed: invalid build.");
                    IsAuthorized = false;
                }
            }

            public DebugLogger Log
            {
                get
                {
                    return _log;
                }
                private set
                {
                    _log = value;
                }
            }
        }
    }
}
