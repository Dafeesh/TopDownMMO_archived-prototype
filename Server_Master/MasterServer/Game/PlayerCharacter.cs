using System;
using System.Collections.Generic;

using SharedComponents.Server.World;
using SharedComponents.Server;

namespace MasterServer.Game
{
    public class PlayerCharacter
    {
        public PlayerCharacterInfo Info
        { get; private set; }
        public WorldLocation Location
        { get; private set; }

        public PlayerCharacter(PlayerCharacterInfo info, WorldLocation worldLocation)
        {
            this.Info = info;
            this.Location = worldLocation;
        }
    }
}
