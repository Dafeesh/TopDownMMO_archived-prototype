using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public partial class Characters
    {
        public abstract class Npc : Character
        {
            public Npc(String name, CharacterType type)
                : base("Npc:" + name, type)
            {

            }
        }
    }
}
