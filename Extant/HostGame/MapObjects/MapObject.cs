using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.HostGame.MapObjects
{
    public struct Vec2
    {
        public double x;
        public double y;

        public Vec2()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }

        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vec2))
                return false;

            Vec2 o = (Vec2)obj;
            return ((o.x == this.x) && (o.y == this.y));
        }

        public static readonly Vec2 Zero = new Vec2(0.0f, 0.0f);
    }

    class MapObject
    {
        private Vec2 position;

        protected MapObject(Vec2 position)
        {
            this.position = position;
        }

        public Vec2 Position
        {
            get
            {
                return position;
            }
        }
    }
}
