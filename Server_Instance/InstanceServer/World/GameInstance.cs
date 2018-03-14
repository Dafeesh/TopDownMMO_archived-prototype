using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using InstanceServer.World;
using InstanceServer.World.Map;
using InstanceServer.World.Map.Character;

using SharedComponents.Global;
using InstanceServer.Control;
using SharedComponents.Global.Game.Character;
using SharedComponents.Global.Game;

namespace InstanceServer.World
{
    public abstract class GameInstance : ThreadRun, IInstanceTick
    {
        private string name;
        private GameMap map;
        private List<GameCharacter> allCharacters = new List<GameCharacter>(); //Contains all characters.
        private List<Characters.Npc> npcs = new List<Characters.Npc>(); // Contains just Npc characters.
        private List<Characters.Player> players = new List<Characters.Player>(); //Contains just Player characters.
        private int characterID_iterator = 1;
        private GameTime gameTime = new GameTime();

        private List<Characters.Player> newPlayers = new List<Characters.Player>();
        private object newPlayers_lock = new object();

        private Queue<Characters.Player> loggedOutPlayers = new Queue<Characters.Player>();
        private object loggedOutPlayers_lock = new object();

        public GameInstance(String name, MapLayout mapLayout)
            : base("Instance-" + name)
        {
            this.name = name;
            this.map = new GameMap(mapLayout);

            Log.MessageLogged += Console.WriteLine;
        }

        sealed protected override void Begin()
        {
            gameTime.Start();
            Log.Log("Started.");
        }

        sealed protected override void RunLoop()
        {
            float frameDiff = gameTime.DiffTime() / 1000.0f;

            HandleNpcs(frameDiff);
            HandlePlayers(frameDiff);
        }

        public abstract void Tick(float frameDiff);


        sealed protected override void Finish(bool success)
        {
            foreach (var c in allCharacters)
            {
                c.Dispose();
            }
            Log.Log("Finished.");
        }

        public override string ToString()
        {
            return name;
        }

        /////////// Private ///////////
        private void ReceiveAndProcessPlayerPackets(Characters.Player plr)
        {
            return;
            /*
            Packet p = null;
            while ((p = plr.GetPacket()) != null)
            {
                switch ((ClientToInstancePackets.PacketType)p.Type)
                {
                    case (ClientToInstancePackets.PacketType.Player_MovementRequest_i):
                        {
                            ClientToInstancePackets.Player_MovementRequest_i pp = p as ClientToInstancePackets.Player_MovementRequest_i;

                            MovePoint[] mps = map.CalculatePath(plr.Position, new Position2D(pp.posx, pp.posy));
                            if (mps.Length > 0)
                                plr.SetMovePointsPath(mps);
                        }
                        break;

                    default:
                        Log.Log("Received invalid packet from player: " + plr.Info.Name);
                        plr.SendPacket(new ClientToInstancePackets.Error_c(ClientToInstancePackets.Error_c.ErrorCode.InvalidPacket));
                        break;
                }
            }
            */
        }

        private void HandleNpcs(float frameDiff)
        {
            foreach (var npc in npcs)
            {
                npc.Tick(frameDiff);
            }
        }

        private void HandlePlayers(float frameDiff)
        {
            foreach (var plr in players)
            {
                ReceiveAndProcessPlayerPackets(plr);
                plr.Tick(frameDiff);
            }
        }

        private int NextCharacterID
        {
            get
            {
                return characterID_iterator++;
            }
        }

        private void RemoveCharacter(GameCharacter c)
        {
            c.RemoveSeenByAllOther();

            allCharacters.Remove(c);
        }

        private void RemovePlayer(Characters.Player p)
        {
            p.RemoveSeenByAllOther();

            allCharacters.Remove(p);
            players.Remove(p);
            Log.Log("Player left instance. [" + p.Info.Name + "]");
        }

        private void UpdateCharacterView(GameCharacter c)
        {
            foreach (var otherChar in allCharacters)
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
                    p.SendPacket(new ClientToInstancePackets.Map_TerrainBlock_c(i, j, block));
                }
            }
        }

        private void AddPlayer(Characters.Player p)
        {
            p.SetID(NextCharacterID);

            allCharacters.Add(p);
            players.Add(p);

            p.SendPacket(new ClientToInstancePackets.Map_Reset_c(map.Terrain.NumBlocksX, map.Terrain.NumBlocksY));
            p.SendPacket(new ClientToInstancePackets.Character_Add_c(p.Id, CharacterType.Player, p.Info.Name));
            p.SendPacket(new ClientToInstancePackets.Character_Position_c(p.Id, p.Position.x, p.Position.y));
            p.SendPacket(new ClientToInstancePackets.Player_SetControl_c(p.Id));
            p.SendPacket(new ClientToInstancePackets.Character_UpdateStats_c(p.Id, p.Stats));

            UpdatePlayerMapView(p);
            UpdateCharacterView(p);

            Log.Log("Player joined instance. [" + p.Info.Name + "]");
        }

        /////////// Protected ///////////
        protected void AddCharacter(GameCharacter c)
        {
            c.SetID(NextCharacterID);

            allCharacters.Add(c);

            UpdateCharacterView(c);
            Log.Log("Character added to instance. [" + c.Id + "]");
        }

        protected GameMap Map
        {
            get
            {
                return map;
            }
        }

        protected List<GameCharacter> CharacterList
        {
            get
            {
                return allCharacters;
            }
        }

        /////////// Public ///////////
        public void AddCharacterToInstance(GameCharacter character, Position2D position = null)
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

        public Characters.Npc[] GetNpcs()
        {
            GameCharacter[] all = allCharacters.ToArray();

            return (all.Where((ch) => (ch is Characters.Npc)) 
                as Characters.Npc[]);
        }

        public Characters.Player[] GetPlayers()
        {
            GameCharacter[] all = allCharacters.ToArray();

            return (all.Where((ch) => (ch is Characters.Player))
                as Characters.Player[]);
        }

        public string Name
        {
            get
            {
                return name;
            }
        }
    }

    public interface IInstanceTick
    {
        void Tick(float frameDiff);
    }
}
