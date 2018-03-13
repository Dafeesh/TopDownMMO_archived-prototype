using System;
using System.Collections.Generic;

using SharedComponents.Global.GameProperties;

namespace SharedComponents.Server.World
{
    public static class WorldZones
    {
        private static Dictionary<ZoneID, ZoneInfo> zones = new Dictionary<ZoneID, ZoneInfo>();

        public enum ZoneID
        {
            MainTest
        }

        static WorldZones()
        {
            //MainTest
            {
                MapLayout l = new MapLayout(100, 100);

                l.NpcSpawns.Add(new NpcSpawnLayout(
                        new Position2D(25, 25),
                        new NpcLayout(new CharacterVisualLayout(CharacterVisualLayout.VisualType.Basic))
                    ));

                zones.Add(ZoneID.MainTest, new ZoneInfo("Main Test Zone", ZoneID.MainTest, l));
            }

            //
            {

            }

            //
            {

            }
        }

        public static ZoneInfo GetZoneInfo(ZoneID zID)
        {
            if (zones.ContainsKey(zID))
            {
                return zones[zID];
            }
            else
            {
                throw new InvalidOperationException("Zone not initialized: " + zID.ToString());
            }
        }
    }

    public class WorldLocation
    {
        public readonly UInt32 WorldNumber;
        public readonly WorldZones.ZoneID ZoneId;
        public readonly Position2D Position;

        public WorldLocation(UInt32 worldNumber, WorldZones.ZoneID zid, Position2D pos)
        {
            this.WorldNumber = worldNumber;
            this.ZoneId = zid;
            this.Position = pos;
        }
    }
}
