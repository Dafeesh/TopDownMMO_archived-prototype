using System;

using SharedComponents.Global.GameProperties;

namespace SharedComponents.Server
{
    public class PlayerCharacterInfo
    {
        public readonly string Name;

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
