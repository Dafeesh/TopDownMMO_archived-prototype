using System;
using System.Collections.Generic;

using SharedComponents.Global.GameProperties;

namespace SharedComponents.Server.World
{
    public class ZoneInfo
    {
        public readonly string Name;
        public readonly ZoneID ZoneID;
        public readonly MapLayout MapLayout;

        public ZoneInfo(string name, ZoneID id, MapLayout ml)
        {
            this.Name = name;
            this.ZoneID = id;
            this.MapLayout = ml;
        }
    }
}
