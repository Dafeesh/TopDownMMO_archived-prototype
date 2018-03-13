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
        private Stopwatch stateTime = new Stopwatch();

        public Game_Survival(String gameId, Player gameOwner)
            : base(gameId, gameOwner, new Game_Presets(100, DayPhase.Day, 10000, 10000))
        {

        }

        protected override void OnBegin()
        {
            stateTime.Start();
            DebugLogger.GlobalDebug.LogGame(this.gameId, this.GameTime, "Game has begun!");
        }

        protected override void OnUpdate()
        {
            //REMOVED DAYPHASE TIME CHECK
        }

        protected override void OnFinish()
        {
            DebugLogger.GlobalDebug.LogGame(this.gameId, this.GameTime, "Game has ended.");
        }
    }
}
