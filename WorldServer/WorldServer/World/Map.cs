using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public class Map
    {
        private MapID id;
        private int sizeX, sizeY;
        private Dictionary<int, Position2D> entryPoint;

        public Map(MapID id, int sizeX, int sizeY)
        {
            this.id = id;
            this.sizeX = sizeX;
            this.sizeY = sizeY;

            entryPoint = new Dictionary<int, Position2D>();
        }

        protected void AddEntryPoint(int id, Position2D point)
        {
            if (entryPoint.ContainsKey(id))
                throw new InvalidOperationException("EntryPoint already exists on this map.");
            if (point.x < 0.0f || point.y < 0.0f ||
                point.x > sizeX || point.y > sizeY)
                throw new InvalidOperationException("EntryPoint cannot be outsite the map's range.");

            entryPoint.Add(id, point);
        }

        public Position2D EntryPoint(int id)
        {
            if (entryPoint.ContainsKey(id))
                return entryPoint[id];
            else
                throw new InvalidOperationException("Could not find entryPoint.");
        }

        public MapID Id
        {
            get
            {
                return id;
            }
        }
    }
}
