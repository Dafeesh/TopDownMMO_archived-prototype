using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.GameProperties
{
    public class Position2D
    {
        public float x { set; get; }
        public float y { set; get; }

        public Position2D()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }

        public Position2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(Position2D p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "(" + x.ToString("F") + "," + y.ToString("F") + ")";
        }
    }
}
