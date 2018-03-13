using System;

using SharedComponents.Global.Game;
using SharedComponents.Server.Game.World;
using SharedComponents.Global.Game.Character;

namespace SharedComponents.Server
{
    public class PlayerCharacterInfo
    {
        public readonly string Name;

        public WorldLocation Location;

        public CharacterVisualLayout VisualLayout;
        public Int32 Level;
        public Int32 Level_Progress;
        public Int32 Credits;

        public PlayerCharacterInfo(string name)
        {
            this.Name = name;
        }
    }

}
