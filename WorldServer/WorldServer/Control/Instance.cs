using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using SharedComponents;
using SharedComponents.GameProperties;
using WorldServer.World;
using WorldServer.World.MapItems;
using Extant.Networking;

namespace WorldServer.Control
{
    public abstract class Instance : ThreadRun, IInstanceTick
    {
        private string name;
        private Map map;
        private List<Character> characters; //Contains all characters
        private List<Characters.Player> players; //Contains just player characters
        private int characterID_counter = 1;

        public Instance(String name, MapLayout mapLayout)
            : base("Instance-" + name)
        {
            this.name = name;
            this.map = new Map(mapLayout);

            characters = new List<Character>();
            players = new List<Characters.Player>();

            Log.MessageLogged += Console.WriteLine;
        }

        sealed protected override void Begin()
        {
            Log.Log("Instance started.");
        }

        sealed protected override void RunLoop()
        {
            this.Tick();
        }

        sealed protected override void Finish(bool success)
        {
            foreach (var c in characters)
            {
                c.Dispose();
            }
            Log.Log("Instance finished.");
        }

        public virtual void Tick()
        {
            //Do all Character actions
            for (int i = characters.Count - 1; i >= 0; i--)
            {
                Character c = characters[i];

                c.Tick();
            }

            //Handle players
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Characters.Player plr = players[i];

                ProcessPlayerCommands(plr);

                if (plr.IsLoggingOut)
                {
                    RemovePlayer(plr);
                }
            }
        }

        /////////// Private ///////////
        private void ProcessPlayerCommands(Characters.Player p)
        {
            PlayerCommand command = null;
            while ((command = p.GetCommand()) != null)
            {
                if (command is PlayerCommand.MoveTo)
                {
                    PlayerCommand.MoveTo cmd = (PlayerCommand.MoveTo)command;

                    //log.Log(plr.Info.Name + " moved to (" + cmd.point.x + "," + cmd.point.y + ")");
                    //plr.TeleportTo(cmd.point.x, cmd.point.y);

                    MovePoint[] mps = map.CalculatePath(p.Position, cmd.point);
                    if (mps.Length > 0)
                        p.SetMovePointsPath(mps);
                }
            }
        }

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
            Log.Log("Player left instance. [" + p.Info.Name + "]");
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

        private void UpdatePlayerMapView(Characters.Player p)
        {
            //Give whole map for now
            Single[,] block;
            for (int i = 0; i < map.Terrain.NumBlocksX; i++)
            {
                for (int j = 0; j < map.Terrain.NumBlocksY; j++)
                {
                    block = map.Terrain.GetBlock(i, j);
                    p.SendPacket(new ClientToWorldPackets.Map_TerrainBlock_c(i, j, block));
                }
            }
        }

        private void AddPlayer(Characters.Player p)
        {
            p.SetID(NextCharacterID);
            characters.Add(p);
            players.Add(p);

            p.SendPacket(new ClientToWorldPackets.Map_Reset_c(map.Terrain.NumBlocksX, map.Terrain.NumBlocksY));
            p.SendPacket(new ClientToWorldPackets.Character_Add_c(p.Id, CharacterType.Player, 1));
            p.SendPacket(new ClientToWorldPackets.Character_Position_c(p.Id, p.Position.x, p.Position.y));
            p.SendPacket(new ClientToWorldPackets.Player_SetControl_c(p.Id));

            UpdatePlayerMapView(p);
            UpdateCharacterView(p);

            Log.Log("Player joined instance. [" + p.Info.Name + "]");
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
