using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedComponents.Server.World
{
    public class ZoneInfo
    {
        public readonly string Name;
        public readonly WorldZones.ZoneID ZoneID;
        public readonly MapLayout MapLayout;

        public ZoneInfo(string name, WorldZones.ZoneID id, MapLayout ml)
        {
            this.Name = name;
            this.ZoneID = id;
            this.MapLayout = ml;
        }
    }
}
