using System;
using System.Collections.Generic;

using SharedComponents.Server.World;

namespace MasterServer.Game
{
    public class GameInstance
    {
        public readonly String Name;
        public readonly MapLayout MapLayout;
        public readonly List<PlayerCharacter> ActivePlayerCharacters = new List<PlayerCharacter>();

        public GameInstance(string name, MapLayout mapLayout)
        {
            this.Name = name;
            this.MapLayout = mapLayout;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
