using System;
using System.Collections.Generic;
using System.Net;

using SharedComponents.ServerToServer;
using Extant.Networking;

namespace MasterServer
{
    class GameServerLink
    {
        private string name;
        private IPEndPoint expectedRemoteEndPoint;
        private List<AccountInfo> activeCharacters = new List<AccountInfo>();

        private NetConnection connection = null;

        public GameServerLink(string name, IPEndPoint expectedRemoteEndPoint)
        {
            this.name = name;
            this.expectedRemoteEndPoint = expectedRemoteEndPoint;
        }
    }
}
