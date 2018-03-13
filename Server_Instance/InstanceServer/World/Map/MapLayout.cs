using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.Global.GameProperties;

namespace InstanceServer.World.Map
{
    public struct MapLayout
    {
        private readonly Terrain terrain;

        public MapLayout(Terrain terrain)
        {
            this.terrain = terrain;
        }

        public int SizeX
        {
            get
            {
                return terrain.NumBlocksX * MapDefaults.TERRAINBLOCK_WIDTH;
            }
        }

        public int SizeY
        {
            get
            {
                return terrain.NumBlocksY * MapDefaults.TERRAINBLOCK_WIDTH;
            }
        }

        public Terrain Terrain
        {
            get
            {
                return terrain;
            }
        }
    }

    public enum MapID
    {
        TestMap
    }
}
