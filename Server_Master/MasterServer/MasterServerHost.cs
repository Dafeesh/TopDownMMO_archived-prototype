using System;
using System.Collections.Generic;
using System.Net;

using MasterServer.Links;

using Extant;
using Extant.Networking;

namespace MasterServer
{
    public class MasterServerHost : ThreadRun
    {
        private ForwardServerLink forwardServer;
        private InstanceServerLink[] zoneInstanceServers;
        private InstanceServerLink[] instanceServers;

        public MasterServerHost(ForwardServerLink fwsLink, InstanceServerLink[] zoneInstServLinks, InstanceServerLink[] instServerLinks)
            : base("MasterServer")
        {
            this.forwardServer = fwsLink;
            this.zoneInstanceServers = zoneInstServLinks;
            this.instanceServers = instServerLinks;
        }

        protected override void Begin()
        {
            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            PollConnections();

            HandleForwardServerPackets();
        }

        protected override void Finish(bool success)
        {
            Log.Log("Finish.");
        }

        private void PollConnections()
        {
            forwardServer.PollConnection();

            foreach (InstanceServerLink z in zoneInstanceServers)
            {
                z.PollConnection();
            }
            foreach (InstanceServerLink i in instanceServers)
            {
                i.PollConnection();
            }
        }

        private void HandleForwardServerPackets()
        {
            if (forwardServer.ConnectedState == true)
            {
                Packet packet = null;
                while ((packet = forwardServer.GetPacket()) != null)
                {

                }
            }
        }
    }
}
