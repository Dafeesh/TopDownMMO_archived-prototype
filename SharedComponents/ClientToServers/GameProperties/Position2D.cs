using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.GameProperties
{
    public class Position2D
    {
        public double x { set; get; }
        public double y { set; get; }

        public Position2D()
        {
            this.x = 0.0f;
            this.y = 0.0f;
        }

        public Position2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(Position2D p)
        {
            this.x = p.x;
            this.y = p.y;
        }

        public void Set(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
