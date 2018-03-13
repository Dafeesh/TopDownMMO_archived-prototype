using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.World.MapItems
{
    public class MapLayoutBuilder
    {
        Single[,] heightMap;

        public MapLayoutBuilder(int sizeX, int sizeY)
        {
            heightMap = new Single[sizeX, sizeY];
        }

        public void SetHeight(int x, int y, Single value)
        {
            heightMap[x, y] = value;
        }

        public MapLayout GetLayout()
        {
            return new MapLayout(heightMap);
        }
    }
}
