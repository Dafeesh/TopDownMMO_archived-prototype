using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedComponents.GameProperties;
using WorldServer.World.InstanceItems;

namespace WorldServer.Networking
{
    public class ExpectedPlayer
    {
        private const Int32 TIMEOUT = 10000;

        private Characters.Player.Info info;
        private Stopwatch timeoutTimer = new Stopwatch();

        public ExpectedPlayer(Characters.Player.Info info)
        {
            this.info = info;
            timeoutTimer.Start();
        }

        public Characters.Player.Info PlayerInfo
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
