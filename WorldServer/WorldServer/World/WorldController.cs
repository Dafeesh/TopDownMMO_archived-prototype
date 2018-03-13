using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using SharedComponents.GameProperties;
using WorldServer.Networking;
using WorldServer.World;
using WorldServer.World.InstanceItems;

namespace WorldServer.World
{
    public class WorldController : ThreadRun
    {
        private Dictionary<string, Map> maps = new Dictionary<string, Map>();
        private Dictionary<Instances.Zone.ZoneIDs, Instances.Zone> instances_zone = new Dictionary<Instances.Zone.ZoneIDs, Instances.Zone>();
        private List<Instance> instances_temp = new List<Instance>();
        private List<Characters.Player> players = new List<Characters.Player>();

        private Queue<Characters.Player.Info> loggedPlayers = new Queue<Characters.Player.Info>();
        private object loggedPlayers_lock = new object();

        public WorldController()
            : base("WorldController")
        {

        }

        protected override void Begin()
        {
            LoadMaps();
            CreateZones();
        }

        protected override void RunLoop()
        {
            CheckPlayerLogouts();
        }

        protected override void Finish(bool success)
        {
            foreach (var i in instances_temp)
            {
                i.Dispose();
            }
            foreach (var i in instances_zone)
            {
                i.Value.Dispose();
            }
            foreach (var p in players)
            {
                p.Dispose();
            }
        }

        private void CheckPlayerLogouts()
        {
            players.RemoveAll(plr =>
            {
                if (plr.IsLoggingOut)
                {
                    lock (loggedPlayers_lock)
                    {
                        loggedPlayers.Enqueue(new Characters.Player.Info(plr.Username, plr.Password, plr.ZoneLocation));
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        private void LoadMaps()
        {
            maps.Add("TestMap", new Maps.TestMap());
        }

        private void CreateZones()
        {
            Instances.Zone testZone = new Instances.Zone(Instances.Zone.ZoneIDs.TestZone, maps["TestMap"]);
            testZone.Start();
            instances_zone.Add(Instances.Zone.ZoneIDs.TestZone, testZone);

            Character c1 = new Characters.RandomTeleportingWizard(100, 100);
            Character c2 = new Characters.RandomTeleportingWizard(150, 100);
            testZone.AddCharacterToInstance(c1);
            testZone.AddCharacterToInstance(c2);
        }

        ////////// Public //////////
        public void AddPlayer(Characters.Player.Info info, ClientConnection con)
        {
            this.Invoke(() =>
            {
                Characters.Player newPlayer = new Characters.Player(info);
                newPlayer.SetClient(con);

                players.Add(newPlayer);
                instances_zone[info.Location.Zone].AddCharacterToInstance(newPlayer);
            });
        }

        public Characters.Player.Info GetLoggedPlayer()
        {
            Characters.Player.Info info = null;
            lock (loggedPlayers_lock)
            {
                if (loggedPlayers.Count > 0)
                {
                    info = loggedPlayers.Dequeue();
                }
            }
            return info;
        }

        public List<Instance> GetInstances()
        {
            List<Instance> insts = new List<Instance>();
            foreach (var z in instances_zone.Values)
            {
                insts.Add(z as Instance);
            }

            insts.AddRange(instances_temp);

            return insts;
        }
    }
}
