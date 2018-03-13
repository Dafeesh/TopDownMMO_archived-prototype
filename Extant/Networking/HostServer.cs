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

        private TcpListener listener;
        private List<Client> newClients;

        private List<Client> varifiedClients;
        private object       varifiedClients_lock = new object();

        public HostServer(String ip, Int32 port)
            : base("<HostServer>")
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            newClients = new List<Client>();
            varifiedClients = new List<Client>();

            listener.Start(TCPLISTENER_MAX_BACKLOG);

            DebugLogger.GlobalDebug.LogNetworking("Started HostServer on: " + ip + "/" + port.ToString());
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
            lock (varifiedClients_lock)
            {
                foreach (Client c in varifiedClients)
                    if (!c.IsStopped)
                        c.Stop();
                varifiedClients = null;
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
                if (newClients[i].IsStopped)
                    newClients.Remove(newClients[i]);
                else if (newClients[i].IsConnected)
                {
                    lock (varifiedClients_lock)
                    {
                        varifiedClients.Add(newClients[i]);
                    }
                    newClients.Remove(newClients[i]);
                }
            }
        }

        private void HandleVerifiedClients()
        {
            //See if state of client has changed.
            if (varifiedClients.Count > 0)
            {
                lock (varifiedClients_lock)
                {
                    if (varifiedClients.Count > 0)
                    {
                        for (int i = 0; i < varifiedClients.Count; i++)
                        {
                            if (varifiedClients[i].IsStopped)
                            {
                                varifiedClients.Remove(varifiedClients[i]);
                            }
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
            if (varifiedClients.Count > 0)
            {
                lock (varifiedClients_lock)
                {
                    if (varifiedClients.Count > 0)
                    {
                        c = varifiedClients[0];
                        varifiedClients.Remove(c);
                    }
                }
            }
            return c;
        }
    }
}
