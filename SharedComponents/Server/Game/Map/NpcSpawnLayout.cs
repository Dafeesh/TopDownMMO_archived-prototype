using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.Global.GameProperties;
using SharedComponents.Server.Game.Character;

namespace SharedComponents.Server.Game.Map
{
    public class NpcSpawnLayout
    {
        public readonly Position2D Point;
        public readonly NpcLayout NpcLayout;

        public NpcSpawnLayout(Position2D point, NpcLayout layout)
        {
            this.Point = point;
            this.NpcLayout = layout;
        }
    }
}
