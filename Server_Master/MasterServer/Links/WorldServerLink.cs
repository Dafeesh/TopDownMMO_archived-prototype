using System;
using System.Collections.Generic;
using System.Net;

using Extant;
using SharedComponents.Server.World;
using MasterServer.Game;
using SharedComponents.Global.GameProperties;

namespace MasterServer.Links
{
    public class WorldServerLink : IDisposable, ILogging
    {
        public UInt32 WorldNumber
        { get; private set; }
        public InstanceServerLink ServerLink
        { get; private set; }

        private Dictionary<ZoneID, GameInstance> zones = new Dictionary<ZoneID, GameInstance>();

        private DebugLogger _log;
        private bool _isDisposed = false;

        public WorldServerLink(UInt32 worldNumber, InstanceServerLink instLink)
        {
            this.Log = new DebugLogger("World:" + worldNumber);
            this.Log.MessageLogged += Console.WriteLine;

            this.WorldNumber = worldNumber;
            this.ServerLink = instLink;

            foreach (ZoneID zid in Enum.GetValues(typeof(ZoneID)))
            {
                ZoneInfo zoneInfo = WorldZones.GetZoneInfo(zid);
                GameInstance zoneInstance = new GameInstance(zoneInfo.Name, zoneInfo.MapLayout);

                ServerLink.AddInstance(zoneInstance);
            }

            Log.Log("Start.");
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                ServerLink.Dispose();

                Log.Log("Disposed.");
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }
            private set
            {
                _log = value;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
            private set
            {
                _isDisposed = value;
            }
        }

        public override string ToString()
        {
            return "World Server " + WorldNumber;
        }
    }
}
