using System;
using System.Collections.Generic;

using Extant;

using SharedComponents.Server.World;

using MasterServer.Links;

namespace MasterServer.Host
{
    public class ServerHub : IDisposable, ILogging
    {
        private WorldServerLink[] worlds;
        private InstanceServerLink[] instances;

        private DebugLogger _log = new DebugLogger("InstHub");
        private bool _isDisposed = false;

        public ServerHub(WorldServerLink[] worldServers, InstanceServerLink[] instanceServers)
        {
            this.Log.MessageLogged += Console.WriteLine;

            this.worlds = worldServers;
            this.instances = instanceServers;

            Log.Log("Start.");
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                foreach (var w in worlds)
                {
                    w.Dispose();
                }
                foreach (var i in instances)
                {
                    i.Dispose();
                }

                Log.Log("Disposed.");
            }
        }

        public void PollServers()
        {
            foreach (var w in worlds)
            {
                w.ServerLink.PollConnection();
            }
            foreach (var i in instances)
            {
                i.PollConnection();
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
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
    }
}
