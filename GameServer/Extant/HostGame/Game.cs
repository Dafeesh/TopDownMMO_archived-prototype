using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Extant;
using Extant.Networking;
using Extant.GameServerShared;

using GameServer.Networking;

namespace GameServer.HostGame
{
    public abstract partial class Game : ThreadRun
    {
        private readonly String gameId;
        private List<Player> players = new List<Player>();

        private GameState gameState;
        private GameTime gameTime = new GameTime();
        private Game_Presets presets;
        private DayPhase phase;
        private Stopwatch phaseTime = new Stopwatch();

        protected Game(String gameId, Player[] players, Game_Presets presets)
            : base("Game")
        {
            this.gameId = gameId;
            this.presets = presets;
            this.phase = presets.startingDayPhase;

            this.players.AddRange(players);

            DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "Game created: " + gameId + " - " + (players.Length).ToString() + "p");
        }

        /// Triggered when phase changes in the game.
        protected abstract void OnPhaseChange(DayPhase newPhase);
        /// Triggered when game goes into Play state.
        protected abstract void OnBegin();
        /// Triggered once every cycle while in Play state.
        protected abstract void OnUpdate();
        /// Triggered when game was terminated.
        protected abstract void OnFinish();

        protected override void Begin()
        {
            GameState = GameState.Play; //This will be changed later because the hub server will dictate when it is ready.

            this.phaseTime.Start();
        }

        protected override void RunLoop()
        {
            if (gameState == GameState.Play)
            {
                HandleDayPhase();
                HandlePlayers();
                OnUpdate();
            }
        }

        protected override void Finish()
        {
            OnFinish();
            DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "Game stopped. (" + gameId + ")");
        }

        private void HandleDayPhase()
        {
            switch (Phase)
            {
                case(DayPhase.Day):
                    {
                        if (phaseTime.ElapsedMilliseconds > presets.dayPhase_day_timeLength)
                        {
                            Phase = DayPhase.Night;
                        }
                        break;
                    }
                case (DayPhase.Night):
                    {
                        if (phaseTime.ElapsedMilliseconds > presets.dayPhase_night_timeLength)
                        {
                            Phase = DayPhase.Day;
                        }
                        break;
                    }
            }
        }

        private void HandlePlayers()
        {
            foreach (Player p in players)
            {
                //Check if newly connected
                if (p.IsNewlyConnected)
                {
                    //Send GameInfo
                    p.SendPacket(new GameServerPackets.Game_Info_c(0, gameTime.ElapsedMilliseconds));
                    p.SendPacket(new GameServerPackets.Game_DayPhase_c((int)phase));
                    //Inform of all the current players
                    foreach (Player op in players)
                    {
                        p.SendPacket(new GameServerPackets.Player_Add_c(op.Username, op.Clan, op.Level, op.ModelNumber));
                    }
                    //Inform which player client is in control of
                    //p.SendPacket(new Packets.Player_SetControl_c());

                    p.IsNewlyConnected = false;
                    DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "Sent gameinfo");
                }

                //HandlePackages
                Packet newPacket = null;
                if ((newPacket = p.GetPacket()) != null)
                {
                    switch ((GameServerPackets.PacketType)newPacket.Type)
                    {
                        case (GameServerPackets.PacketType.Player_Movement_g):
                            {
                                DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "Player_Movement_s");

                                break;
                            }
                        default:
                            {
                                DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "Player sent unsupported packet: " + newPacket.Type.ToString());
                                p.Disconnect();

                                break;
                            }
                    }
                }
            }
        }

        private GameState GameState
        {
            set
            {
                gameState = value;
                DebugLogger.GlobalDebug.LogGame(gameId, gameTime.ElapsedMilliseconds, "GameState set to " + gameState.ToString() + ".");

                if (gameState == GameState.Play)
                {
                    gameTime.Start();
                    OnBegin();
                }
            }
            get
            {
                return gameState;
            }
        }

        /// <summary>
        /// Returns the Game Time in milliseconds.
        /// </summary>
        public Int32 GameTime
        {
            get
            {
                return (Int32)gameTime.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Sets the game's DayPhase.
        /// </summary>
        /// <param name="p"></param>
        protected DayPhase Phase
        {
            set
            {
                phase = value;
                phaseTime.Restart();
                Packet dpPacket = new GameServerPackets.Game_DayPhase_c((Int32)phase);
                foreach (Player p in Players)
                {
                    p.SendPacket(dpPacket);
                }

                OnPhaseChange(phase);
            }

            get
            {
                return phase;
            }
        }

        /// <summary>
        /// Returns a readonly reference to the list of players.
        /// </summary>
        public System.Collections.ObjectModel.ReadOnlyCollection<Player> Players
        {
            get
            {
                return players.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds a player to the game and informs the other players.
        /// </summary>
        public void AddPlayer(Player player)
        {
            players.Add(player);
            Packet newPlayerPacket = new GameServerPackets.Player_Add_c(player.Username, player.Clan, player.Level, player.ModelNumber);
            foreach (Player p in players)
            {
                p.SendPacket(newPlayerPacket);
            }
            DebugLogger.GlobalDebug.LogGame(this.gameId, gameTime.ElapsedMilliseconds, "Player added to game: " + player.Username);
        }

        /// <summary>
        /// Returns the string identification of the game.
        /// </summary>
        public String GameID
        {
            get
            {
                return gameId;
            }
        }
    }

    /// <summary>
    /// First tier GameStates. Used as the basics for any type of game.
    /// Only seen serverside.
    /// </summary>
    public enum GameState
    {
        Waiting, //Waiting for hub server to dictate it as ready.
        Play, //Able to be connected to and played.
        Finished //Completed and has a report ready to be sent to hub server.
    }

    /// <summary>
    /// Constant properties set for a game to use as presets.
    /// </summary>
    public struct Game_Presets
    {
        public Game_Presets( Int32 bsh, DayPhase sdp, Int32 dp_d_t, Int32 dp_n_t )
        {
            baseStartingHealth = bsh;
            startingDayPhase = sdp;
            dayPhase_day_timeLength = dp_d_t;
            dayPhase_night_timeLength = dp_n_t;
        }

        public readonly Int32 baseStartingHealth;

        public readonly DayPhase startingDayPhase;
        public readonly Int32 dayPhase_day_timeLength;
        public readonly Int32 dayPhase_night_timeLength;
    }

    public struct Game_Report
    {
        /// <summary>
        /// Used to be a standard way of reporting games, no matter the GameType.
        /// </summary>
        public struct Game_Report_PlayerReport
        {
            public String username;
        }
    }
}
