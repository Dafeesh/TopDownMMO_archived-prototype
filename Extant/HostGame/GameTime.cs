using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.HostGame
{
    class GameTime
    {
        private Int32 startTime;
        private Stopwatch gameTime = new Stopwatch();

        /// <summary>
        /// Elapsed time with a starting value.
        /// </summary>
        /// <param name="time">The starting time (in milliseconds).</param>
        public GameTime(Int32 time = 0)
        {
            this.startTime = time;
        }

        /// <summary>
        /// Returns the total amount of time, in milliseconds, since the creation of this object added to the start time.
        /// </summary>
        public Int32 ElapsedMilliseconds
        {
            get
            {
                return (Int32)(startTime + gameTime.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            gameTime.Start();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            if (gameTime.IsRunning)
                gameTime.Stop();
        }
    }
}
