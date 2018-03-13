using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

using Extant;
using Extant.Networking;
using SharedComponents.ServerToServer;

namespace WorldServer.Networking
{
    public class MasterServerLink : ThreadRun
    {
        private NetConnection connection = null;
        private IPEndPoint targetEndPoint;

        public MasterServerLink(IPEndPoint targetEndPoint)
            : base("MastServLink", 5)
        {
            this.targetEndPoint = targetEndPoint;
        }

        protected override void Begin()
        { 
            
        }

        protected override void RunLoop()
        {
            if (IsConnected)
            {

            }
        }

        protected override void Finish(bool success)
        {
            if (connection != null)
                connection.Stop("Dispose on Finish from MasterServerLink.");
        }

        private void AttemptConnect()
        {
            NetConnection con = new NetConnection(WorldToMasterPackets.ReadBuffer, targetEndPoint, 5000);

            //if (Start here)
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
    }
}
