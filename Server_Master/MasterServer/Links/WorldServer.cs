using System;
using System.Collections.Generic;
using System.Net;

using Extant;

using SharedComponents.Global.Game;

namespace MasterServer.Links
{
    public class WorldServer : ServerLink
    {
        public Int32 WorldNumber
        { get; private set; }

        public WorldServer(Int32 worldNumber, IPEndPoint remoteEndPoint, IPEndPoint broadcastEndPoint)
            : base("World-" + worldNumber, remoteEndPoint, broadcastEndPoint)
        {
            this.WorldNumber = worldNumber;

            Log.Log("Start.");
        }
    }
}
