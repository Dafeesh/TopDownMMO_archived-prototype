using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.GameProperties
{
    public struct PlayerInfo
    {
        public PlayerInfo(string name)
        {
            Name = name;

            Level = -1;
        }

        public string Name;
        public int Level;
    }
}
