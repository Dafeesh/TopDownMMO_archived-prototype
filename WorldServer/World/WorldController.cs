using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using WorldServer.Networking;
using WorldServer.World.MapItems;
using SharedComponents.GameProperties;

namespace WorldServer.World
{
    class WorldController : ThreadRun
    {
        private List<Map> maps = new List<Map>();
        private List<Player> players = new List<Player>();

        public WorldController()
            : base("WorldController")
        {

        }

        protected override void Begin()
        {

        }

        protected override void RunLoop()
        {

        }

        protected override void Finish(bool success)
        {

        }

        public void AddPlayer(PlayerInfo info, ClientConnection con)
        {
            this.Invoke(new Action(() =>
            {
                players.Add(new Player(info, con));
            }));
        }
    }
}
