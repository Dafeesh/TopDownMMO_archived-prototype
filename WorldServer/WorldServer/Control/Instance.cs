using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using WorldServer.World;
using WorldServer.World.MapItems;
using WorldServer.Control.InstanceItems;
using SharedComponents;
using SharedComponents.GameProperties;
using Extant.Networking;

namespace WorldServer.Control
{
    public abstract class Instance : ThreadRun, IInstanceTick
    {
        private string name;
        private Map map;
        private List<Character> characters; //Contains all characters
        private List<Characters.Player> players; //Contains just characters that are a player
        private int characterID_iterator = 1;
        private GameTime gameTime = new GameTime();

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
            gameTime.Start();
        }

        sealed protected override void RunLoop()
        {
            float frameDiff = gameTime.DiffTime() / 1000.0f;

            //Do all Character actions
            for (int i = characters.Count - 1; i >= 0; i--)
            {
                Character c = characters[i];

                c.Tick(frameDiff);
            }

            //Handle players
            for (int i = players.Count - 1; i >= 0; i--)
            {
                Characters.Player plr = players[i];

                ReceiveAndProcessPlayerPackets(plr);

                if (plr.IsLoggingOut)
                {
                    RemovePlayer(plr);
                }
            }
        }

        public abstract void Tick(float frameDiff);


        sealed protected override void Finish(bool success)
        {
            foreach (var c in characters)
            {
                c.Dispose();
            }
            Log.Log("Instance finished.");
        }

        public override string ToString()
        {
            return name;
        }

        /////////// Private ///////////
        private void ReceiveAndProcessPlayerPackets(Characters.Player plr)
        {
            Packet p = null;
            while ((p = plr.GetPacket()) != null)
            {
                switch ((ClientToWorldPackets.PacketType)p.Type)
                {
                    case (ClientToWorldPackets.PacketType.Player_MovementRequest_w):
                        {
                            ClientToWorldPackets.Player_MovementRequest_w pp = p as ClientToWorldPackets.Player_MovementRequest_w;

                            MovePoint[] mps = map.CalculatePath(plr.Position, new Position2D(pp.posx, pp.posy));
                            if (mps.Length > 0)
                                plr.SetMovePointsPath(mps);
                        }
                        break;

                    default:
                        Log.Log("Received invalid packet from player: " + plr.Info.Name);
                        plr.SendPacket(new ClientToWorldPackets.Error_c(ClientToWorldPackets.Error_c.ErrorCode.InvalidPacket));
                        break;
                }
            }
        }

        private int NextCharacterID
        {
            get
            {
                return characterID_iterator++;
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
            p.SendPacket(new ClientToWorldPackets.Character_UpdateStats_c(p.Id, p.Stats));

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
