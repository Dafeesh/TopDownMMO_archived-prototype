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
    public class ClientAccepter : ThreadRun
    {
        private const Int32 TCPLISTENER_MAX_BACKLOG = 10;
        private const Int32 NEWCLIENT_TIMEOUT = 5000;

        private TcpListener listener;
        private List<VerifyingClient> newClients;

        private Queue<VerifyingClient> verifiedClients;
        private object verifiedClients_lock = new object();

        public ClientAccepter(IPEndPoint localEndPoint, Int32 maxBacklog = TCPLISTENER_MAX_BACKLOG, Int32 newClientTimeout = NEWCLIENT_TIMEOUT)
            : base("HostServer")
        {
            listener = new TcpListener(localEndPoint);
            newClients = new List<VerifyingClient>();
            verifiedClients = new Queue<VerifyingClient>();
        }

        protected override void Begin()
        {
            listener.Start(TCPLISTENER_MAX_BACKLOG);

            Log.Log("Started on: "
                    + (listener.Server.LocalEndPoint as IPEndPoint).Address
                    + "/" + (listener.Server.LocalEndPoint as IPEndPoint).Port);
        }

        protected override void RunLoop()
        {
            AcceptNewTcpClients();
            HandleNewClients();
            HandleVerifiedClients();
        }

        protected override void Finish(bool success)
        {
            listener.Stop();
            foreach (VerifyingClient c in newClients)
                c.Dispose();
            lock (verifiedClients_lock)
            {
                foreach (VerifyingClient c in verifiedClients)
                    c.Dispose();
            }

            Log.Log("Stopped.");
        }

        private void AcceptNewTcpClients()
        {
            while (listener.Pending())
            {
                newClients.Add(new VerifyingClient(listener.AcceptTcpClient()));
                DebugLogger.Global.Log("Client joined.");
            }
        }

        private void HandleNewClients()
        {
            newClients.RemoveAll((cl) =>
            {
                if (!cl.IsConnected)
                {
                    cl.Dispose();
                    return true;
                }
                else if (cl.IsVerified)
                {
                    lock (verifiedClients_lock)
                    {
                        verifiedClients.Enqueue(cl);
                    }
                    return true;
                }
                else
                    return false;
            });
        }

        private void HandleVerifiedClients()
        {
            lock (verifiedClients_lock)
            {
                newClients.RemoveAll((cl) =>
                {
                    return !(cl.IsConnected);

                });
            }
        }

        public VerifyingClient GetVerifiedClient()
        {
            VerifyingClient c = null;
            lock (verifiedClients_lock)
            {
                if (verifiedClients.Count > 0)
                {
                    c = verifiedClients.Dequeue();
                }
            }
            return c;
        }
    }
}
