using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Extant;

using SharedComponents;
using SharedComponents.GameProperties;
using WorldServer.World;
using WorldServer.Control;

namespace WorldServer.Networking
{
    class WorldHost : ThreadRun
    {
        private WorldController worldController;
        private ClientAccepter clientAccepter;

        private List<ExpectedPlayer> expectedPlayers = new List<ExpectedPlayer>();

        public WorldHost(IPEndPoint localEndPoint)
            : base("Program")
        {
            clientAccepter = new ClientAccepter(localEndPoint);
            worldController = new WorldController();
        }

        protected override void Begin()
        {
            clientAccepter.Start();
            worldController.Start();

            //TEST
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Template(new PlayerInfo("Player1") { Level = 1 }, 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(10, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Template(new PlayerInfo("Player2") { Level = 2 }, 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(30, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Template(new PlayerInfo("Player3") { Level = 3 }, 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(50, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Template(new PlayerInfo("Player4") { Level = 4 }, 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(70, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            //~
        }

        protected override void RunLoop()
        {
            HandleNewClients();
            HandlePlayerLogout();
        }

        protected override void Finish(bool success)
        {
            clientAccepter.Dispose();
            worldController.Dispose();

            //TODO: Send account info to main
        }

        private void HandleNewClients()
        {
            ClientConnection c;
            while ((c = clientAccepter.GetVarifiedClient()) != null)
            {
                bool found = false;
                for (int i = 0; i < expectedPlayers.Count; i++)
                {
                    if (c.VerifyUsername == expectedPlayers[i].PlayerInfo.Info.Name)
                    {
                        found = true;
                        if (c.VerifyPasswordToken == expectedPlayers[i].PlayerInfo.Password)
                        {
                            c.SendPacket(new ClientToWorldPackets.Verify_Result_c(ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.Success));
                            worldController.AddPlayer(expectedPlayers[i].PlayerInfo, c);
                            expectedPlayers.Remove(expectedPlayers[i]);

                            DebugLogger.Global.Log("Player logged in: " + c.VerifyUsername);
                        }
                        else
                        {
                            c.SendPacket(new ClientToWorldPackets.Verify_Result_c(ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.IncorrectPassword));
                        }
                    }
                }

                if (!found)
                {
                    c.SendPacket(new ClientToWorldPackets.Verify_Result_c(ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.DoesNotExist));
                }
            }
        }

        private void HandlePlayerLogout()
        {
            Characters.Player.Template p;
            while ((p = worldController.GetLoggedPlayer()) != null)
            {
                DebugLogger.Global.Log("Player logged out: " + p.Info.Name);
            }
        }

        public WorldController WorldController
        {
            get
            {
                return worldController;
            }
        }
    }
}
