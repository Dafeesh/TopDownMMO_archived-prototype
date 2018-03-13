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
using WorldServer.World.InstanceItems;

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
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Info("Player1", 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(10, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Info("Player2", 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(30, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Info("Player3", 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(50, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            expectedPlayers.Add(new ExpectedPlayer(new Characters.Player.Info("Player4", 111, new Characters.Player.PlayerZoneLocation() { Position = new Position2D(70, 10), Zone = Instances.Zone.ZoneIDs.TestZone })));
            //~
        }

        protected override void RunLoop()
        {
            HandleNewClients();
            HandlePlayerLogout();
        }

        protected override void Finish(bool success)
        {
            clientAccepter.Stop();
            worldController.Stop();

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
                    if (c.VerifyUsername == expectedPlayers[i].PlayerInfo.Username)
                    {
                        found = true;
                        if (c.VerifyPassword == expectedPlayers[i].PlayerInfo.Password)
                        {
                            c.SendPacket(new ClientToWorldPackets.Verify_Result_c(ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.Success));
                            worldController.AddPlayer(expectedPlayers[i].PlayerInfo, c);
                            expectedPlayers.Remove(expectedPlayers[i]);

                            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Player logged in: " + c.VerifyUsername);
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
            Characters.Player.Info p;
            while ((p = worldController.GetLoggedPlayer()) != null)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Player logged out: " + p.Username);
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
