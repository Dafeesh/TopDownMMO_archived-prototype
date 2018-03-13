using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

using SharedComponents.Server.Game.Character;
using SharedComponents.Server.Game.Map;

namespace SharedComponents.Server.Game.Instance
{
    public abstract class GameInstance : ILogging
    {
        private string _name;

        private GameMap map;
        private List<GameCharacter> characters = new List<GameCharacter>(); //Contains all characters.
        private List<GameCharacter.Npc> characters_npcs = new List<GameCharacter.Npc>(); // Contains just Npc characters.
        private List<GameCharacter.Player> characters_players = new List<GameCharacter.Player>(); //Contains just Player characters.

        private Queue<GameCharacter.Player> loggedOutPlayers = new Queue<GameCharacter.Player>();

        private int characterID_iterator = 1;
        private GameTime gameTime = new GameTime();

        private DebugLogger _log;

        public GameInstance(String name, MapLayout mapLayout)
        {
            this.Name = name;
            this.map = new GameMap(mapLayout);

            this.Log = new DebugLogger("Inst:" + name);
            this.Log.MessageLogged += Console.WriteLine;

            gameTime.Start();
            Log.Log("Started.");
        }

        public void Tick()
        {
            float frameDiff = gameTime.DiffTime() / 1000.0f;

            HandleNpcs(frameDiff);
            HandlePlayers(frameDiff);
        }

        public PlayerCharacterInfo[] Stop()
        {
            Log.Log("Finished.");

            return new PlayerCharacterInfo[0];
        }

        public override string ToString()
        {
            return Name;
        }

        /////////// Private ///////////
        private void HandleNpcs(float frameDiff)
        {
            foreach (var npc in characters_npcs)
            {
                npc.Tick(frameDiff);
            }
        }

        private void HandlePlayers(float frameDiff)
        {
            foreach (var plr in characters_players)
            {
                //ReceiveAndProcessPlayerPackets(plr);
                plr.Tick(frameDiff);
            }
        }

        private int NextCharacterID
        {
            get { return characterID_iterator++; }
        }

        private void UpdateCharacterView(GameCharacter c)
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

        private void UpdatePlayerMapView(GameCharacter.Player p)
        {
            throw new NotImplementedException();

            /*
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
             * */
        }

        /////////// Public ///////////

        public void AddNpc(GameCharacter.Npc npc, Position2D position = null)
        {
            if (position != null)
                npc.Position.Set(position);

            npc.SetID(NextCharacterID);

            characters.Add(npc);
            characters_npcs.Add(npc);

            Log.Log("Added npc: " + npc.Id);

            UpdateCharacterView(npc);
        }

        public void RemoveNpc(GameCharacter.Npc npc)
        {
            npc.RemoveSeenByAllOther();

            characters_npcs.Remove(npc);
            characters.Remove(npc);

            Log.Log("Removed npc: " + npc.Id);
        }

        public void AddPlayer(GameCharacter.Player p, Position2D position = null)
        {
            if (position != null)
                p.Position.Set(position);

            p.SetID(NextCharacterID);

            characters.Add(p);
            characters_players.Add(p);

            //p.SendPacket(new ClientToInstancePackets.Map_Reset_c(map., map.Terrain.NumBlocksY));
            p.SendPacket(new ClientToInstancePackets.Character_Add_c(p.Id, CharacterType.Player, p.Info.Name));
            p.SendPacket(new ClientToInstancePackets.Character_Position_c(p.Id, p.Position.x, p.Position.y));
            p.SendPacket(new ClientToInstancePackets.Player_SetControl_c(p.Id));
            p.SendPacket(new ClientToInstancePackets.Character_UpdateStats_c(p.Id, p.Stats));

            UpdatePlayerMapView(p);
            UpdateCharacterView(p);

            Log.Log("Added player: " + p.Info.Name + "/" + p.Id);
        }

        public void RemovePlayer(GameCharacter.Player p)
        {
            p.RemoveSeenByAllOther();

            characters_players.Remove(p);
            characters.Remove(p);

            Log.Log("Removed player: " + p.Info.Name + "/" + p.Id);
        }

        public IEnumerable<GameCharacter> Characters
        {
            get
            {
                return characters;
            }
        }

        public IEnumerable<GameCharacter.Npc> Npcs
        {
            get
            {
                return characters_npcs;
            }
        }

        public IEnumerable<GameCharacter.Player> Players
        {
            get
            {
                return characters_players;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }
            private set
            {
                _log = value;
            }
        }
    }

    public interface IInstanceTick
    {
        void Tick(float frameDiff);
    }
}