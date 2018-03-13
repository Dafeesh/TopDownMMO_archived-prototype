using System;

namespace SharedComponents.Global.GameProperties
{
    public class MovePoint
    {
        public readonly Position2D start;
        public readonly Position2D end;

        public MovePoint(float startX, float startY, float endX, float endY)
        {
            start = new Position2D(startX, startY);
            end = new Position2D(endX, endY);
        }

        public MovePoint(Position2D start, Position2D end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
