using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Extant;

using GameServer.Networking;
using GameServer.Shared;

namespace GameServer.HostGame
{
    public abstract partial class Game : ThreadRun
    {
        protected readonly String gameId;
        private List<Player> players = new List<Player>();

        private GameState gameState;
        private Stopwatch gameTime = new Stopwatch();
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

            DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Game created. (" + this.RunningID + ")");
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
            //GameState = GameState.Waiting;
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
            DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Game stopped. (" + this.RunningID + ")");
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
                if (p.NewlyConnected)
                {
                    p.SendPacket(new GameInfo_p(0));
                    p.SendPacket(new DayPhase_p((int)phase, (int)phaseTime.Elapsed.TotalMilliseconds, (int)gameTime.Elapsed.TotalMilliseconds));
                    p.NewlyConnected = false;
                    DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Sent gameinfo");
                }

                //HandlePackages
                Packet newPacket = null;
                if ((newPacket = p.GetPacket()) != null)
                {
                    switch (newPacket.Type)
                    {
                        case (Packet.PacketType.Ping_sp):
                            {
                                DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Ping (" + (newPacket as Ping_sp).num + ")");
                                p.SendPacket(new Ping_sp(Ping_sp.ECHO));

                                break;
                            }
                        case (Packet.PacketType.Player_Movement_s):
                            {
                                DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Player_Movement_s");

                                break;
                            }
                        default:
                            {
                                DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Player sent incorrect packet: " + newPacket.Type.ToString());
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
                DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "GameState set to " + gameState.ToString() + ".");

                if (gameState == GameState.Play)
                {
                    if (gameTime.IsRunning)
                    {
                        gameTime.Reset();
                    }
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
        protected Int32 GameTime
        {
            get
            {
                return (Int32)gameTime.Elapsed.TotalMilliseconds;
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
                Packet dpPacket = new DayPhase_p((Int32)phase, (Int32)phaseTime.Elapsed.TotalMilliseconds, (Int32)gameTime.Elapsed.TotalMilliseconds);
                foreach (Player p in Players)
                {
                    p.SendPacket(dpPacket);
                }
                //DebugLogger.GlobalDebug.LogGame(gameId, (int)gameTime.Elapsed.TotalMilliseconds, "Phase set to " + phase.ToString() + ".");
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
