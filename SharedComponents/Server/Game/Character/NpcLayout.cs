using System;
using System.Collections.Generic;

using SharedComponents.Global.Game.Character;

namespace SharedComponents.Server.Game.Character
{
    public class NpcLayout
    {
        public CharacterVisualLayout VisualLayout;

        public NpcLayout(CharacterVisualLayout vlayout)
        {
            this.VisualLayout = vlayout;
        }
    }
}
