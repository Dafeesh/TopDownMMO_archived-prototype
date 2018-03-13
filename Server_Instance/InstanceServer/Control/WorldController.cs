using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;

using SharedComponents;
using SharedComponents.GameProperties;
using WorldServer.Networking;
using WorldServer.World;
using WorldServer.World.MapItems;

namespace WorldServer.Control
{
    public class WorldController : ThreadRun
    {
        private Dictionary<MapID, MapLayout> maps = new Dictionary<MapID, MapLayout>();
        private Dictionary<Instances.Zone.ZoneIDs, Instances.Zone> instances_zone = new Dictionary<Instances.Zone.ZoneIDs, Instances.Zone>();
        private List<Instance> instances_temp = new List<Instance>();
        private List<Characters.Player> players = new List<Characters.Player>();

        private Queue<Characters.Player.Template> loggedPlayers = new Queue<Characters.Player.Template>();
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
                        loggedPlayers.Enqueue(new Characters.Player.Template(plr.Info, plr.Password, plr.ZoneLocation));
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
            int nBlocksX = 5;
            int nBlocksY = 5;

            //Build maps
            MapLayoutBuilder testMap = new MapLayoutBuilder(nBlocksX, nBlocksY);
            for (int i = 0; i < nBlocksX * MapDefaults.TERRAINBLOCK_WIDTH; i++)
            {
                for (int j = 0; j < nBlocksY * MapDefaults.TERRAINBLOCK_WIDTH; j++)
                {
                    testMap.SetHeight(i, j, Math.Abs((float)((i + j) % 10) - 5));
                }
            }

            maps.Add(MapID.TestMap, testMap.GetLayout());
        }

        private void CreateZones()
        {
            Instances.Zone testZone = new Instances.Zone(Instances.Zone.ZoneIDs.TestZone, maps[MapID.TestMap]);


            Character c1 = new Characters.RandomTeleportingWizard(15, 15);
            Character c2 = new Characters.RandomTeleportingWizard(15, 10);
            testZone.AddCharacterToInstance(c1);
            testZone.AddCharacterToInstance(c2);


            testZone.Start();
            instances_zone.Add(Instances.Zone.ZoneIDs.TestZone, testZone);
        }

        ////////// Public //////////
        public void AddPlayer(Characters.Player.Template template, ClientConnection con)
        {
            this.Invoke(() =>
            {
                Characters.Player newPlayer = new Characters.Player(template);
                newPlayer.SetClient(con);
                newPlayer.SendPacket(new ClientToWorldPackets.Player_Info_c(newPlayer.Info.Name, newPlayer.Info.Level));

                players.Add(newPlayer);
                instances_zone[template.Location.Zone].AddPlayerToInstance(newPlayer);
            });
        }

        public Characters.Player[] GetPlayerList()
        {
            return players.ToArray();
        }

        public Characters.Player.Template GetLoggedPlayer()
        {
            Characters.Player.Template info = null;
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
