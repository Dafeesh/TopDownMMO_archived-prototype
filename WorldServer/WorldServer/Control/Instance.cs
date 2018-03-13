using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using SharedComponents;
using SharedComponents.GameProperties;
using WorldServer.World;

namespace WorldServer.Control
{
    public abstract class Instance : ThreadRun, IInstanceTick
    {
        private string name;
        private Map map;
        private List<Character> characters; //Contains all characters
        private List<Characters.Player> players; //Contains just player characters
        private int characterID_counter = 1;

        private DebugLogger log;

        public Instance(String name, MapLayout map)
            : base("Instance-" + name)
        {
            this.name = name;
            this.map = new Map(map);

            characters = new List<Character>();
            players = new List<Characters.Player>();

            log = new DebugLogger();
            log.AnyLogged += Console.WriteLine;
        }

        sealed protected override void Begin()
        {
            log.Log("Instance started: " + this.name);
        }

        sealed protected override void RunLoop()
        {
            this.Tick();

            //Do all Character actions
            for (int i = characters.Count - 1; i >= 0; i--)
            {
                Character c = characters[i];

                c.Tick();
            }

            //Handle players
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Characters.Player p = players[i];

                if (p.IsLoggingOut)
                {
                    RemovePlayer(p);
                }
            }
        }

        sealed protected override void Finish(bool success)
        {
            foreach (var c in characters)
            {
                c.Dispose();
            }
            log.Log("Instance finished: " + this.name);
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
            c.RemoveSeenByAllOther();

            characters.Remove(c);
        }

        private void RemovePlayer(Characters.Player p)
        {
            p.RemoveSeenByAllOther();

            characters.Remove(p);
            players.Remove(p);
            log.Log("Player left instance. [" + p.Info.Name + " -> " + this.name + "]");
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

        private void AddPlayer(Characters.Player p)
        {
            p.SetID(NextCharacterID);
            characters.Add(p);
            players.Add(p);

            p.SendPacket(new ClientToWorldPackets.Map_Reset_c());
            //Send immediate map pieces
            p.SendPacket(new ClientToWorldPackets.Character_Add_c(p.Id, CharacterType.Player, 1));
            p.SendPacket(new ClientToWorldPackets.Character_Position_c(p.Id, p.Position.x, p.Position.y));
            p.SendPacket(new ClientToWorldPackets.Player_SetControl_c(p.Id));
            UpdateCharacterView(p);

            log.Log("Player joined instance. [" + p.Info.Name + " -> " + this.name + "]");
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

        protected List<Character> CharacterList
        {
            get
            {
                return characters;
            }
        }

        /////////// Public ///////////
        public void AddCharacterToInstance(Character character, Position2D position = null)
        {
            if (position != null)
                character.Position.Set(position);

            this.Invoke(() =>
            {
                this.AddCharacter(character);
            });
        }

        public void AddPlayerToInstance(Characters.Player p, Position2D position = null)
        {
            if (position != null)
                p.Position.Set(position);

            this.Invoke(() =>
            {
                this.AddPlayer(p);
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
