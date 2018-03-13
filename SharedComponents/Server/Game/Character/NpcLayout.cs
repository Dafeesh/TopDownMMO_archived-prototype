using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.Global.GameProperties;

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
