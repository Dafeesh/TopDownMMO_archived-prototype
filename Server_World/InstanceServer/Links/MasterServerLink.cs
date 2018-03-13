using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using Extant;
using Extant.Networking;
using SharedComponents.Server;
using SharedComponents.Global;

namespace InstanceServer.Links
{
    public class MasterServerLink : ThreadRun
    {
        public event Delegate_MasterServerLinkStateChanged StateChanged;
        public delegate void Delegate_MasterServerLinkStateChanged(ConnectionState state);

        private ConnectionState _state;
        private TcpListener listener;
        private NetConnection connection = null;
        private object connection_lock = new object();
        private IPAddress expectedIPAddress;

        public MasterServerLink(IPEndPoint localEndPoint, IPAddress expectedIPAddress)
            : base("MastServLink", 50)
        {
            this.expectedIPAddress = expectedIPAddress;
            this.listener = new TcpListener(localEndPoint);

            this.Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            listener.Start();

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            if (State == ConnectionState.Connected)
            {
                if (!IsConnected)
                {
                    CloseConnection();
                }
            }
            else
            {
                AcceptConnection();
            }
        }

        protected override void Finish(bool success)
        {
            listener.Stop();
            CloseConnection();

            StateChanged = null;

            Log.Log("Finished.");
        }

        private void AcceptConnection()
        {
            if (listener.Pending())
            {
                TcpClient possibleClient = listener.AcceptTcpClient();

                if ((possibleClient.Client.RemoteEndPoint as IPEndPoint).Address.Equals(expectedIPAddress))
                {
                    SetAsConnection(possibleClient);
                    Log.Log("Master server connected!");
                }
                else
                {
                    possibleClient.Close();
                    Log.Log("Invalid endpoint from MasterServer connection.");
                }
            }
        }

        private void SetAsConnection(TcpClient con)
        {
            lock (connection_lock)
            {
                if (connection != null)
                    connection.Dispose();

                connection = new NetConnection(con);
                connection.Start();
                State = ConnectionState.Connected;
            }
        }

        public void DistributePackets(IPacketDistributor distributor)
        {
            lock (connection_lock)
            {
                if (IsConnected)
                {
                    while (connection.DistributePacket(distributor) == true)
                    { }
                }
            }
        }

        public void CloseConnection()
        {
            lock (connection_lock)
            {
                if (IsConnected)
                {
                    connection.Dispose();
                    State = ConnectionState.NoConnection;
                }
            }
        }

        public ConnectionState State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value != _state)
                {
                    _state = value;
                    if (StateChanged != null)
                        StateChanged(_state);
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                if (connection != null)
                    return connection.State == NetConnection.NetworkState.Active;
                else
                    return false;
            }
        }

        public enum ConnectionState
        {
            NoConnection,
            Connected
        }
    }
}
