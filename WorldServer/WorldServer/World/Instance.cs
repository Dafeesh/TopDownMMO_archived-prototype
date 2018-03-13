using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using SharedComponents.GameProperties;
using WorldServer.World.InstanceItems;

namespace WorldServer.World
{
    public abstract class Instance : ThreadRun , IInstanceTick
    {
        private string name;
        private Map map;
        private List<Character> characters;
        private int characterID_counter = 1;

        public Instance(String name, Map map)
            : base("Instance-" + name)
        {
            this.name = name;
            this.map = map;

            characters = new List<Character>();
        }

        sealed protected override void Begin()
        {

        }

        sealed protected override void RunLoop()
        {
            this.Tick();
            for (int i=0; i<characters.Count; i++)
            {
                Character c = characters[i];

                c.Tick();

                if (c is Characters.Player)
                    if ((c as Characters.Player).IsLoggingOut)
                        RemoveCharacter(c);
            }
        }

        sealed protected override void Finish(bool success)
        {
            foreach (var c in characters)
            {
                c.Dispose();
            }
        }

        public abstract void Tick();

        /////////// Private ///////////
        private int NextCharacterID
        {
            get
            {
                return characterID_counter++;
            }
        }

        private void RemoveCharacter(Character c)
        {
            foreach (Character otherChar in characters)
            {
                if (otherChar != c)
                    c.RemoveSeenByAllOther();
            }

            characters.RemoveAll((ch) =>
            {
                if (ch == c)
                    return true;
                else
                    return false;
            });
        }

        private void UpdateCharacterView(Character c)
        {
            foreach (var otherChar in characters)
            {
                if (otherChar != c)
                {
                    if (otherChar.CanSeeCharacter(c))
                    {
                        otherChar.AddCharacterInView(c);
                        c.AddCharacterSeenBy(otherChar);
                    }

                    if (c.CanSeeCharacter(otherChar))
                    {
                        c.AddCharacterInView(otherChar);
                        otherChar.AddCharacterSeenBy(c);
                    }
                }
            }
        }

        /////////// Protected ///////////
        protected void AddCharacter(Character c)
        {
            c.SetID(NextCharacterID);
            characters.Add(c);

            UpdateCharacterView(c);
        }

        protected Map Map
        {
            get
            {
                return map;
            }
        }

        protected List<Character> Characters
        {
            get
            {
                return characters;
            }
        }

        /////////// Public ///////////
        public void AddCharacterToInstance(Character c, int entryPoint = -1)
        {
            if (entryPoint >= 0)
            {
                c.Position.Set(map.EntryPoint(entryPoint));
            }

            this.Invoke(() =>
            {
                this.AddCharacter(c);
            });
        }

        public List<Character> GetCharacters()
        {
            return characters.ToArray().ToList();
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
