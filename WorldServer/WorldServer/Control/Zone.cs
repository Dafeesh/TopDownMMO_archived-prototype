using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WorldServer.World;
using WorldServer.World.MapItems;

namespace WorldServer.Control
{
    public partial class Instances
    {
        public class Zone : Instance
        {
            private ZoneIDs id;

            public Zone(ZoneIDs id, MapLayout mapLayout)
                : base(id.ToString(), mapLayout)
            {
                this.id = id;
            }

            public override void Tick()
            {
                base.Tick();


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
