﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InstanceServer.World.Map;
using SharedComponents.Global.Game;

namespace InstanceServer.World
{
    public sealed class GameMap
    {
        private MapLayout layout;

        public GameMap(MapLayout layout)
        {
            this.layout = layout;
        }

        public MovePoint[] CalculatePath(Position2D start, Position2D finish)
        {
            List<MovePoint> mp = new List<MovePoint>();

            //No pathfinding for now
            mp.Add(new MovePoint(start, finish));

            return mp.ToArray();
        }

        public int SizeX
        {
            get
            {
                return layout.SizeX;
            }
        }

        public int SizeY
        {
            get
            {
                return layout.SizeY;
            }
        }

        public Terrain Terrain
        {
            get
            {
                return layout.Terrain;
            }
        }
    }
}
