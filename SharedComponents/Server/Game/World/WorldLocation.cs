using System;
using System.Collections.Generic;

using SharedComponents.Global.Game;

namespace SharedComponents.Server.Game.World
{
    public class WorldLocation
    {
        public readonly UInt32 WorldNumber;
        public readonly ZoneID ZoneId;
        public readonly Position2D Position;

        public WorldLocation(UInt32 worldNumber, ZoneID zid, Position2D pos)
        {
            this.WorldNumber = worldNumber;
            this.ZoneId = zid;
            this.Position = pos;
        }
    }
}
