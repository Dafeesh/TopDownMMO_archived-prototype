using System;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public partial class Maps
    {
        public class TestMap : Map
        {
            public TestMap()
                : base(MapID.TestMap, 200, 200)
            {
                this.AddEntryPoint((int)EntryPointID.Main, new Position2D(20, 20));
            }

            public enum EntryPointID
            {
                Main
            }
        } 
    }
}
