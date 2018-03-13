using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.World
{
    public struct MapLayout
    {
        private readonly Single[,] heightMap;

        public MapLayout(Single[,] heightMap)
        {
            this.heightMap = heightMap;
        }

        public Single GetHeightAt(Single x, Single y)
        {
            if (x - (int)x >= 0.50f)
                x += 1.0f;
            if (y - (int)y >= 0.50f)
                y += 1.0f;

            return heightMap[(int)x, (int)y];
        }

        public int SizeX
        {
            get
            {
                return heightMap.GetLength(0);
            }
        }

        public int SizeY
        {
            get
            {
                return heightMap.GetLength(1);
            }
        }
    }

    public enum MapID
    {
        TestMap
    }
}
