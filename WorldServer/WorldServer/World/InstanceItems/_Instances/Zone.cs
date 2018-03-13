using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.World.InstanceItems
{
    public partial class Instances
    {
        public class Zone : Instance
        {
            private ZoneIDs id;

            public Zone(ZoneIDs id, Map map)
                : base(id.ToString(), map)
            {
                this.id = id;
            }

            public override void Tick()
            {
                
            }

            public ZoneIDs ZoneID
            {
                get
                {
                    return id;
                }
            }

            public enum ZoneIDs
            {
                TestZone
            }
        }
    }
}
