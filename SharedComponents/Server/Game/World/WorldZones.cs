using System;
using System.Collections.Generic;

using SharedComponents.Global.Game;

using SharedComponents.Server.Game.Map;
using SharedComponents.Server.Game.Character;
using SharedComponents.Global.Game.Character;

namespace SharedComponents.Server.Game.World
{
    public static class WorldZones
    {
        private static Dictionary<ZoneID, ZoneInfo> zones = new Dictionary<ZoneID, ZoneInfo>();

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
}
