using System;
using System.Collections.Generic;
using System.Net;

namespace MasterServer.Links
{
    public class GeneralServer : ServerLink
    {
        public Int32 ServerNumber
        { get; private set; }

        public GeneralServer(int serverNumber, IPEndPoint remoteEndPoint, IPEndPoint broadcastEndPoint)
            : base("General-" + serverNumber, remoteEndPoint, broadcastEndPoint)
        {
            this.ServerNumber = serverNumber;

            Log.Log("Start.");
        }
    }
}
