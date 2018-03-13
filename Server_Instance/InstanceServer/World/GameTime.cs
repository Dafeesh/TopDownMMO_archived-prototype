using System;
using System.Diagnostics;

namespace InstanceServer.World
{
    public class GameTime
    {
        Stopwatch stopwatch = new Stopwatch();
        UInt32 diffTimeLast = 0;

        public GameTime(bool startImmediately = false)
        {
            if (startImmediately)
                this.Start();
        }

        public void Start()
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();
        }

        public void Pause()
        {
            if (stopwatch.IsRunning)
                stopwatch.Stop();
        }

        public void Restart()
        {
            this.Pause();
            stopwatch.Reset();
            this.Start();
        }

        public UInt32 ElapsedMilliseconds
        {
            get
            {
                return (UInt32)stopwatch.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Returns the difference in time since the last DiffTime().
        /// </summary>
        public UInt32 DiffTime()
        {
            UInt32 diff = this.ElapsedMilliseconds - diffTimeLast;
            diffTimeLast = this.ElapsedMilliseconds;
            return diff;
        }
    }
}
