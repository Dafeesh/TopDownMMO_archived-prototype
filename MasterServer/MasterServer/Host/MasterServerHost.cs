using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

using Extant;
using Extant.Networking;
using SharedComponents.ServerToServer;

namespace MasterServer.Host
{
    public class MasterServerHost : ThreadRun
    {
        public HostState State
        { get; private set; }

        private TcpListener listener;
        private IPEndPoint localEndPoint;

        private WorldServerLink[] gameServers;
        private List<AuthenticatingClient> authenticatingClients = new List<AuthenticatingClient>();

        public MasterServerHost(IPEndPoint hostLocalEndPoint, WorldServerLink[] gameServers)
            : base("MasterServer")
        {
            this.listener = new TcpListener(hostLocalEndPoint);
            this.localEndPoint = hostLocalEndPoint;
            this.gameServers = gameServers;
            State = HostState.Null;

            Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            listener.Start();
            Log.Log("Started.");
        }

        protected override void RunLoop()
        {
            AcceptNewClients();
            HandleAuthenticatingClients();
        }

        protected override void Finish(bool success)
        {
            listener.Stop();
            Log.Log("Finished.");
        }

        private void AcceptNewClients()
        {
            if (listener.Pending())
            {
                NetConnection newClient = new NetConnection(WorldToMasterPackets.ReadBuffer, listener.AcceptTcpClient());
                newClient.Start();
                AuthenticatingClient newClientAuth = new AuthenticatingClient(newClient);
                newClientAuth.Start();
                authenticatingClients.Add(newClientAuth);

                Log.Log("New client: " + newClient.RemoteEndPoint.Address.ToString() + ":" + newClient.RemoteEndPoint.Port.ToString());
            }
        }

        private void HandleAuthenticatingClients()
        {
            for (int i = authenticatingClients.Count - 1; i >= 0; i--)
            {
                AuthenticatingClient cl = authenticatingClients[i];

                if (cl.State == AuthenticatingClient.AuthenticationStep.Failed)
                {
                    cl.Dispose();
                    authenticatingClients.Remove(cl);
                    Log.Log("Client failed to authenticate: " + cl.StopMessage);
                }
                else if (cl.State == AuthenticatingClient.AuthenticationStep.Success)
                {
                    bool existsInList = false;
                    foreach (WorldServerLink gsl in gameServers)
                    {
                        if (gsl.ExpectedRemoteEndPoint.Equals(cl.IPEndPoint))
                        {
                            existsInList = true;
                            if (cl.Authenticate_Build == SharedComponents.StaticFields.Build.Number)
                            {
                                if (cl.Authenticate_ServerId == gsl.ServerId)
                                {
                                    cl.SendPacket(new WorldToMasterPackets.Authenticate_Response_w(WorldToMasterPackets.Authenticate_Response_w.ResponseCode.Success));
                                    gsl.SetConnection(cl.TakeNetConnection());
                                    cl.Dispose();
                                    authenticatingClients.Remove(cl);
                                    Log.Log("Client successfully authenticated!");
                                }
                                else
                                {
                                    cl.SendPacket(new WorldToMasterPackets.Authenticate_Response_w(WorldToMasterPackets.Authenticate_Response_w.ResponseCode.InvalidServerId));
                                    cl.Dispose();
                                    authenticatingClients.Remove(cl);
                                    Log.Log("Client removed because it had wrong serverId.");
                                }
                            }
                            else
                            {
                                cl.SendPacket(new WorldToMasterPackets.Authenticate_Response_w(WorldToMasterPackets.Authenticate_Response_w.ResponseCode.InvalidBuild));
                                cl.Dispose();
                                authenticatingClients.Remove(cl);
                                Log.Log("Client removed because it had wrong build number.");
                            }
                        }
                    }

                    if (existsInList == false)
                    {
                        cl.Dispose();
                        authenticatingClients.Remove(cl);
                        Log.Log("Client removed because it did not match any expected EndPoints: " + cl.IPEndPoint.Address.ToString() + ":" + cl.IPEndPoint.Port);
                    }
                }
            }
        }
    }

    public enum HostState
    {
        Null,
        Active,
        Failed
    }
}
