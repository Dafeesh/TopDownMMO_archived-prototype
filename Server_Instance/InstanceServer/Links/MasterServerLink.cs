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
        private IPAddress expectedIPAddress;

        public MasterServerLink(IPEndPoint localEndPoint, IPAddress expectedIPAddress)
            : base("MastServLink", 100)
        {
            this.expectedIPAddress = expectedIPAddress;

            listener = new TcpListener(localEndPoint);
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
                if (connection.State != NetConnection.NetworkState.Connected)
                {
                    connection.Dispose();
                    State = ConnectionState.NoConnection;
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
            if (connection != null)
                connection.Dispose();

            Log.Log("Finished.");
        }

        private void AcceptConnection()
        {
            if (listener.Pending())
            {
                TcpClient possibleClient = listener.AcceptTcpClient();

                if ((possibleClient.Client.RemoteEndPoint as IPEndPoint).Address.Equals(expectedIPAddress))
                {
                    connection = new NetConnection(InstanceToMasterPackets.ReadBuffer, possibleClient);
                    connection.Start();
                    State = ConnectionState.Connected;
                    Log.Log("Master server connected!");
                }
                else
                {
                    possibleClient.Close();
                    Log.Log("Invalid endpoint from MasterServer connection.");
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
                    return connection.State == NetConnection.NetworkState.Connected;
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
