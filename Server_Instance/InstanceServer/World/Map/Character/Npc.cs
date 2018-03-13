using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.Global.GameProperties;

namespace InstanceServer.World.Map.Character
{
    public partial class Characters
    {
        public abstract class Npc : GameCharacter
        {
            public Npc(String name, CharacterVisualLayout vlayout)
                : base("Npc:" + name, vlayout)
            {

            }
        }
    }
}
