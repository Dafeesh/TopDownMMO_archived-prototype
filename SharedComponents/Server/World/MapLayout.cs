using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedComponents.Server.World
{
    public class MapLayout
    {
        public readonly Int32 Width;
        public readonly Int32 Height;

        public readonly List<NpcSpawnLayout> NpcSpawns = new List<NpcSpawnLayout>();

        public MapLayout(Int32 Width, Int32 Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
}
