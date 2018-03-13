using System;
using System.Collections.Generic;

using Extant;

using MasterServer.Links;

namespace MasterServer.Host
{
    public class ServerHub : IDisposable, ILogging
    {
        private Dictionary<int, WorldServer> worldServers = new Dictionary<int,WorldServer>();
        private Dictionary<int, GeneralServer> generalServers = new Dictionary<int,GeneralServer>();

        private DebugLogger _log = new DebugLogger("InstHub");
        private bool _isDisposed = false;

        public ServerHub(WorldServer[] ws, GeneralServer[] gs)
        {
            this.Log.MessageLogged += Console.WriteLine;

            //World Servers
            foreach (WorldServer w in ws)
            {
                worldServers.Add(w.WorldNumber, w);
            }

            //General Servers
            foreach (GeneralServer g in gs)
            {
                generalServers.Add(g.ServerNumber, g);
            }

            Log.Log("Start.");
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                foreach (var w in worldServers)
                {
                    w.Value.Dispose();
                }
                foreach (var i in generalServers)
                {
                    i.Value.Dispose();
                }

                Log.Log("Disposed.");
            }
        }

        public ServerState GetWorldServerStatus(int worldNumber)
        {
            if (worldServers.ContainsKey(worldNumber))
            {
                if (worldServers[worldNumber].IsConnected)
                {
                    return ServerState.Online;
                }
                else
                {
                    return ServerState.WorldOffline;
                }
            }
            else
            {
                return ServerState.InvalidServerNumber;
            }
        }

        public WorldServer GetWorldServer(int worldNumber)
        {
            if (worldServers.ContainsKey(worldNumber))
            {
                return worldServers[worldNumber];
            }
            else
            {
                throw new InvalidOperationException("GetWorldServer was directed to an unknown world number: " + worldNumber);
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

        public enum ServerState
        {
            Online,
            WorldOffline,
            InvalidServerNumber
        }
    }
}
