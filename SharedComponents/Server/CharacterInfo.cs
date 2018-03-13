using System;

using SharedComponents.Global.GameProperties;

namespace SharedComponents.Server
{
    public class CharacterInfo
    {
        public readonly string Owner;
        public readonly string Name;

        public CharacterLayout Layout;
        public Int32 Level;
        public Int32 Level_Progress;
        public Int32 Credits;

        public CharacterInfo(string owner, string name)
        {
            this.Owner = owner;
            this.Name = name;
        }
    }

}
