using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.World.MapItems
{
    public class MapLayoutBuilder
    {
        Terrain terrain;

        public MapLayoutBuilder(int numBlocksX, int numBlocksY)
        {
            terrain = new Terrain(numBlocksX, numBlocksY);
        }

        public void SetHeight(int x, int y, Single value)
        {
            terrain.SetHeightAtPoint(x, y, value);
        }

        public MapLayout GetLayout()
        {
            return new MapLayout(terrain);
        }
    }
}
