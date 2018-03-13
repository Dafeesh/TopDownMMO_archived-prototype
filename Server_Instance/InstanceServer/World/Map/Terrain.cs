using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;

namespace InstanceServer.World.Map
{
    public class Terrain
    {
        private int numBlocksX;
        private int numBlocksY;
        private Single[,][,] blocks;

        public Terrain(int numBlocksX, int numBlocksY)
        {
            this.numBlocksX = numBlocksX;
            this.numBlocksY = numBlocksY;

            blocks = new Single[numBlocksX, numBlocksY][,];
            for (int i = 0; i < numBlocksX; i++)
            {
                for (int j = 0; j < numBlocksY; j++)
                {
                    blocks[i, j] = new Single[MapDefaults.TERRAINBLOCK_WIDTH, MapDefaults.TERRAINBLOCK_WIDTH];
                }
            }
        }

        public Single[,] GetBlock(int i, int j)
        {
            if (i < 0 || i > numBlocksX ||
                j < 0 || j > numBlocksY)
            {
                return null;
            }
            else
            {
                return blocks[i, j];
            }
        }

        public void SetHeightAtPoint(int x, int y, Single h)
        {
            if (x < 0 || x >= numBlocksX * MapDefaults.TERRAINBLOCK_WIDTH ||
                y < 0 || y >= numBlocksY * MapDefaults.TERRAINBLOCK_WIDTH)
            {
                return;
            }
            else
            {
                blocks[x / MapDefaults.TERRAINBLOCK_WIDTH, y / MapDefaults.TERRAINBLOCK_WIDTH][x % MapDefaults.TERRAINBLOCK_WIDTH, y % MapDefaults.TERRAINBLOCK_WIDTH] = h;
            }
        }

        public int NumBlocksX
        {
            get
            {
                return numBlocksX;
            }
        }

        public int NumBlocksY
        {
            get
            {
                return numBlocksY;
            }
        }
    }
}
