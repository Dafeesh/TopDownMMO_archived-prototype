using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using MasterServer.Links;

using Extant;

namespace ForwardServer
{
    public class ForwardServerHost : ThreadRun
    {
        private TcpListener listener;
        private MasterServerLink masterServerLink;

        public ForwardServerHost(IPEndPoint clientLocalEndPoint, MasterServerLink masterServerLink)
            : base("FwdServer")
        {
            this.listener = new TcpListener(clientLocalEndPoint);
            this.masterServerLink = masterServerLink;
        }

        protected override void Begin()
        {
            listener.Start();
            Log.Log("Start.");
        }

        protected override void RunLoop()
        {

        }

        protected override void Finish(bool success)
        {
            masterServerLink.Stop("Host stopped.");
            listener.Stop();
            Log.Log("Finish.");
        }
    }
}
