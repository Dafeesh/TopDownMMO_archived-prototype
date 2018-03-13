using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;

namespace WorldServer.Networking
{
    public class ExpectedPlayer
    {
        private const Int32 TIMEOUT = 10000;

        private PlayerInfo info;
        private Stopwatch timeoutTimer = new Stopwatch();

        public ExpectedPlayer(PlayerInfo info)
        {
            this.info = info;
            timeoutTimer.Start();
        }

        public PlayerInfo PlayerInfo
        {
            get
            {
                return info;
            }
        }

        public bool IsTimedOut
        {
            get
            {
                return (timeoutTimer.ElapsedMilliseconds > TIMEOUT);
            }
        }
    }
}
