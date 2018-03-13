using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Extant;

namespace GameServer.Networking
{
    class HostServer : ThreadRun
    {
        private static readonly Int32 TCPLISTENER_MAX_BACKLOG = 10;
        private static readonly Int32 NEWCLIENT_TIMEOUT = 5000;

        private TcpListener listener;
        private List<Client> newClients;

        private List<Client> verifiedClients;
        private object       verifiedClients_lock = new object();

        public HostServer(String ip, Int32 port)
            : base("HostServer")
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            newClients = new List<Client>();
            verifiedClients = new List<Client>();
        }

        protected override void Begin()
        {
            listener.Start(TCPLISTENER_MAX_BACKLOG);

            DebugLogger.GlobalDebug.LogNetworking("Started HostServer on: " 
                                                  + (listener.Server.LocalEndPoint as IPEndPoint).Address 
                                                  + "/" + (listener.Server.LocalEndPoint as IPEndPoint).Port);
        }

        protected override void RunLoop()
        {
            AcceptNewTcpClient();
            HandleNewClients();
            HandleVerifiedClients();
        }

        protected override void Finish()
        {
            listener.Stop();
            listener = null;
            foreach (Client c in newClients)
                if (!c.IsStopped)
                    c.Stop();
            newClients = null;
            lock (verifiedClients_lock)
            {
                foreach (Client c in verifiedClients)
                    if (!c.IsStopped)
                        c.Stop();
                verifiedClients = null;
            }
        }

        private void AcceptNewTcpClient()
        {
            if (listener.Pending())
            {
                Client c = new Client(listener.AcceptTcpClient());
                c.Start();
                newClients.Add(c);
                DebugLogger.GlobalDebug.LogNetworking("Client joined.");
            }
        }

        private void HandleNewClients()
        {
            //See if state of client has changed.
            for (int i = 0; i < newClients.Count; i++)
            {
                //Stopped or timed out?
                if (newClients[i].IsStopped || newClients[i].LifeTime > NEWCLIENT_TIMEOUT)
                {
                    newClients[i].Stop();
                    newClients.Remove(newClients[i]);
                }
                else if (newClients[i].IsConnected) //Connected and verified, add to verified list.
                {
                    lock (verifiedClients_lock)
                    {
                        verifiedClients.Add(newClients[i]);
                    }
                    newClients.Remove(newClients[i]);
                }
            }
        }

        private void HandleVerifiedClients()
        {
            //See if any verified clients have disconnected.
            if (verifiedClients.Count > 0)
            {
                lock (verifiedClients_lock)
                {
                    for (int i = 0; i < verifiedClients.Count; i++)
                    {
                        if (verifiedClients[i].IsStopped)
                        {
                            verifiedClients.Remove(verifiedClients[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dequeues the latest Client to be varified.
        /// </summary>
        /// <returns>The varified client object.</returns>
        public Client GetVarifiedClient()
        {
            Client c = null;
            if (verifiedClients.Count > 0)
            {
                lock (verifiedClients_lock)
                {
                    if (verifiedClients.Count > 0)
                    {
                        c = verifiedClients[0];
                        verifiedClients.Remove(c);
                    }
                }
            }
            return c;
        }
    }
}
