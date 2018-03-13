using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using GameServer.Shared;

namespace GameServer.HostGame
{
    /// <summary>
    /// Non-competitive single-team surivival mode.
    /// </summary>
    public partial class Game_Survival : Game
    {
        public Game_Survival(String gameId, Player[] players)
            : base(gameId, players, new Game_Presets(100, DayPhase.Day, 10000, 10000))
        { }

        protected override void OnPhaseChange(DayPhase newPhase)
        {
            DebugLogger.GlobalDebug.LogGame(this.GameID, this.GameTime, "DayPhase has changed to " + newPhase.ToString() + ".");
        }

        protected override void OnBegin()
        {
            DebugLogger.GlobalDebug.LogGame(this.GameID, this.GameTime, "Game has begun!");
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnFinish()
        {
            DebugLogger.GlobalDebug.LogGame(this.GameID, this.GameTime, "Game has ended!");
        }
    }
}
