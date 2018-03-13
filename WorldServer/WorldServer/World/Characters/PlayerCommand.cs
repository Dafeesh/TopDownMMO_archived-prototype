using System;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public abstract class PlayerCommand
    {
        public class MoveTo : PlayerCommand
        {
            public Position2D point;

            public MoveTo(Position2D point)
            {
                this.point = point;
            }
        }
    }
}
