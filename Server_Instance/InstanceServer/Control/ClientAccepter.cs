using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Extant;

namespace InstanceServer.Control
{
    class ClientAccepter : ThreadRun
    {
        private const Int32 TCPLISTENER_MAX_BACKLOG = 10;
        private const Int32 NEWCLIENT_TIMEOUT = 5000;

        private TcpListener listener;
        private List<ClientAuthConnection> newClients;

        private List<ClientAuthConnection> verifiedClients;
        private object       verifiedClients_lock = new object();

        public ClientAccepter(IPEndPoint localEndPoint, Int32 maxBacklog = TCPLISTENER_MAX_BACKLOG, Int32 newClientTimeout = NEWCLIENT_TIMEOUT)
            : base("HostServer")
        {
            listener = new TcpListener(localEndPoint);
            newClients = new List<ClientAuthConnection>();
            verifiedClients = new List<ClientAuthConnection>();
        }

        protected override void Begin()
        {
            listener.Start(TCPLISTENER_MAX_BACKLOG);

            DebugLogger.Global.Log("Started HostServer on: " 
                                                  + (listener.Server.LocalEndPoint as IPEndPoint).Address 
                                                  + "/" + (listener.Server.LocalEndPoint as IPEndPoint).Port);
        }

        protected override void RunLoop()
        {
            AcceptNewTcpClient();
            HandleNewClients();
            HandleVerifiedClients();
        }

        protected override void Finish(bool success)
        {
            listener.Stop();
            foreach (ClientAuthConnection c in newClients)
                c.Dispose();
            lock (verifiedClients_lock)
            {
                foreach (ClientAuthConnection c in verifiedClients)
                    c.Dispose();
            }
        }

        private void AcceptNewTcpClient()
        {
            if (listener.Pending())
            {
                ClientAuthConnection c = new ClientAuthConnection(listener.AcceptTcpClient());
                c.Start();
                newClients.Add(c);
                DebugLogger.Global.Log("Client joined.");
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
                    newClients[i].Stop("Client timed out.");
                    newClients.Remove(newClients[i]);
                }
                else if (newClients[i].IsAlive)
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
        public ClientAuthConnection GetVarifiedClient()
        {
            ClientAuthConnection c = null;
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
