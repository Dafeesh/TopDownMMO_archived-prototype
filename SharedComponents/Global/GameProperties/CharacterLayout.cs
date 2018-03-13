using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.Global.GameProperties
{
    public class CharacterLayout
    {
        public readonly VisualType Type;

        public CharacterLayout(VisualType vtype)
        {
            this.Type = vtype;
        }

        public enum VisualType
        {
            Null
        }
    }
}
