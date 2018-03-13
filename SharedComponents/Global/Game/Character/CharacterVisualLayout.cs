using System;
using System.Collections.Generic;

namespace SharedComponents.Global.Game.Character
{
    public class CharacterVisualLayout
    {
        public readonly VisualType Type;

        public CharacterVisualLayout(VisualType vtype)
        {
            this.Type = vtype;
        }

        public enum VisualType
        {
            Basic
        }
    }
}
