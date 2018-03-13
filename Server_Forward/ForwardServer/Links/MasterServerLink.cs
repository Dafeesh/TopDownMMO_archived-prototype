using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SharedComponents.Server;

using Extant;
using Extant.Networking;

namespace MasterServer.Links
{
    public class MasterServerLink : ThreadRun
    {
        public event StateChange OnStateChange;
        public delegate void StateChange(MasterServerLink fwdServer);

        private IPAddress expectedIPAddress;
        private TcpListener listener;
        private NetConnection connection = null;
        private object connection_lock = new object();
        private bool _connectedState;

        private object packets_lock = new object();
        private List<ClientIDPacketPair> packets = new List<ClientIDPacketPair>();

        public MasterServerLink(IPEndPoint localEndPoint, IPAddress expectedIPAddress)
            : base("MasterServLink")
        {
            this.expectedIPAddress = expectedIPAddress;
            this.listener = new TcpListener(localEndPoint);
        }

        protected override void Begin()
        {
            listener.Start(1);
            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            if (ConnectedState == true)
            {
                PollConnection();
                DenyAllNewConnections();

                ReceivePackets();
            }
            else//if (ConnectedState == false)
            {
                AcceptConnection();
            }
        }

        protected override void Finish(bool success)
        {
            if (connection != null)
                connection.Stop("MSLink finished.");
            listener.Stop();
            Log.Log("Finish.");
        }

        private void PollConnection()
        {
            if (connection.State == NetConnection.NetworkState.Closed)
                ConnectedState = false;
        }

        private void DenyAllNewConnections()
        {
            if (listener.Pending())
            {
                using (TcpClient c = listener.AcceptTcpClient())
                {
                    Log.Log("Denied connection from: " + c.Client.RemoteEndPoint.ToString() + ". Already have a connnection.");
                }//Dispose
            }
        }

        private void AcceptConnection()
        {
            if (listener.Pending())
            {
                TcpClient s = listener.AcceptTcpClient();

                if ((s.Client.RemoteEndPoint as IPEndPoint).Address.Equals(expectedIPAddress))
                {
                    lock (connection_lock)
                    {
                        connection = new NetConnection(ForwardToMasterPackets.ReadBuffer, s);
                        connection.Start();
                    }

                    ConnectedState = true;
                }
                else
                {
                    Log.Log("Denied connection from: " + s.Client.RemoteEndPoint.ToString());
                    s.Client.Dispose();
                    s.Close();
                }
            }
        }

        private void ReceivePackets()
        {
            Packet p = null;
            while ((p = connection.GetPacket()) != null)
            {
                if (p is ForwardToMasterPackets.AccountAuthorize_Response_f)
            }
        }

        public IEnumerable<Packet> GetPackets(UInt32 clientID)
        {
            lock (packets_lock)
            {
                List<Packet> returnPackets = new List<Packet>();

                int numMatched = packets.RemoveAll((cpacket) =>
                {
                    if (cpacket.ClientID == clientID)
                    {
                        returnPackets.Add(cpacket.Packet);
                        return true;
                    }
                    else
                        return false;
                });

                return returnPackets;
            }
        }

        public void SendPacket(Packet p)
        {
            lock (connection_lock)
            {
                if (connection != null)
                {
                    connection.SendPacket(p);
                }
            }
        }

        public bool ConnectedState
        {
            get
            {
                return _connectedState;
            }

            private set
            {
                _connectedState = value;

                if (OnStateChange != null)
                    OnStateChange(this);
            }
        }
    }
}
