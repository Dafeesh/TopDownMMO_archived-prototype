using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public sealed class Map
    {
        private MapLayout layout;
        private int sizeX, sizeY;

        public Map(MapLayout layout)
        {
            this.layout = layout;
            this.sizeX = layout.SizeX;
            this.sizeY = layout.SizeY;
        }
    }
}
