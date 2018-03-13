using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.Global.GameProperties
{
    public struct PlayerInfo
    {
        public PlayerInfo(string name = "NULL", int level = -1)
        {
            Name = name;
            Level = level;
        }

        public string Name;
        public int Level;
    }
}
