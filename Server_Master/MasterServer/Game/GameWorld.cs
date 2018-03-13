using System;
using System.Collections.Generic;

using MasterServer.Host;
using MasterServer.Links;

using Extant;

namespace MasterServer.Game
{
    public class GameWorld : ILogging , IDisposable
    {
        private InstanceServerHub instanceHub;

        private List<PlayerCharacter> activePlayerCharacters = new List<PlayerCharacter>();

        private DebugLogger _log = new DebugLogger("GameWorld");
        private bool _isDisposed = false;

        public GameWorld(InstanceServerHub hub)
        {
            this.instanceHub = hub;

            Log.Log("Start.");
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                instanceHub.Dispose();

                Log.Log("Disposed.");
            }
        }

        public void PollServers()
        {
            instanceHub.PollServers();
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
