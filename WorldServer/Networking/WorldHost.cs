using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Extant;

using WorldServer.World;
using SharedComponents;
using SharedComponents.GameProperties;

namespace WorldServer.Networking
{
    class WorldHost : ThreadRun
    {
        //private MainServerConnection mainServerConnection;
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
            expectedPlayers.Add(new ExpectedPlayer(new PlayerInfo("Player1", 111)));
            expectedPlayers.Add(new ExpectedPlayer(new PlayerInfo("Player2", 222)));
            expectedPlayers.Add(new ExpectedPlayer(new PlayerInfo("Player3", 333)));
            expectedPlayers.Add(new ExpectedPlayer(new PlayerInfo("Player4", 444)));
            //~
        }

        protected override void RunLoop()
        {
            HandleNewClients();
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
    }
}
