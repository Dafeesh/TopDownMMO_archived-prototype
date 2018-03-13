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
        List<TimedNetConnection> connections = new List<TimedNetConnection>();
        Queue<AuthorizedLoginAttempt> authorizedLoginAttempts = new Queue<AuthorizedLoginAttempt>();
        object authorizedLoginAttempts_lock = new object();
        DBConnection dbConnection = new DBConnection();

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
                c.Connection.Stop("ClientAcceptor finished.");
            }

            listener.Stop();

            Log.Log("Finished.");
        }

        private void HandleConnections()
        {
            //Delete if true...
            connections.RemoveAll((c) =>
            {
                if (!HandleConnection(c))
                {
                    return true;
                }
                return false;
            });
        }

        private bool HandleConnection(TimedNetConnection c)
        {
            //Check for dead connection
            if (c.Connection.IsStopped || c.Timer.ElapsedMilliseconds > RECEIVE_TIMEOUT)
            {
                c.Connection.Dispose();
                c.Timer.Stop();
                return false;
            }
            else
            {
                //Handle packets
                Packet packet = null;
                while ((packet = c.Connection.GetPacket()) != null)
                {
                    if (packet is ClientToMasterPackets.AccountAuthorize_Attempt_m)
                    {
                        var p = packet as ClientToMasterPackets.AccountAuthorize_Attempt_m;

                        if (p.Build == GameVersion.Build)
                        {
                            AccountInfo acc = dbConnection.Fetch_AccountInfo(p.Username, p.Password);
                            if (acc != null)
                            {
                                lock (authorizedLoginAttempts_lock)
                                {
                                    authorizedLoginAttempts.Enqueue(new AuthorizedLoginAttempt(acc, c.Connection));
                                }
                                c.Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.Success));
                                return false;
                            }
                            else
                            {
                                c.Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidLogin));
                                c.Connection.Stop("Login attempt failed.");
                                return false;
                            }
                        }
                        else
                        {
                            c.Connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidBuild));
                            c.Connection.Stop("Invalid build.");
                        }
                    }
                    else
                    {
                        c.Connection.Stop("Received wrong packet while authorizing.");
                    }
                }
                return true;
            }
        }

        private void AcceptConnections()
        {
            while (listener.Pending())
            {
                NetConnection c = new NetConnection(ClientToMasterPackets.ReadBuffer, listener.AcceptTcpClient());
                c.Start();
                connections.Add(new TimedNetConnection(c));
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

        private class TimedNetConnection
        {
            public readonly NetConnection Connection;
            public readonly Stopwatch Timer;

            public TimedNetConnection(NetConnection c)
            {
                this.Connection = c;

                this.Timer = new Stopwatch();
                Timer.Start();
            }
        }
    }
}
