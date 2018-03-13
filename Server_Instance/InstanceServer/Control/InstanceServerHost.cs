using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Extant;

using SharedComponents.Global;
using SharedComponents.Global.GameProperties;
using InstanceServer.World;
using InstanceServer.Control;
using InstanceServer.Links;

namespace InstanceServer.Control
{
    class InstanceServerHost : ThreadRun
    {
        private List<GameInstance> instances = new List<GameInstance>();

        private ClientAccepter clientAccepter;
        private MasterServerLink masterServerLink;

        public InstanceServerHost(ClientAccepter clientAccepter, MasterServerLink masterServerLink)
            : base("InstServHost")
        {
            this.clientAccepter = clientAccepter;
            this.masterServerLink = masterServerLink;
        }

        protected override void Begin()
        {
            clientAccepter.Start();
            masterServerLink.Start();

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            HandleNewClients();
        }

        protected override void Finish(bool success)
        {
            clientAccepter.Dispose();
            masterServerLink.Dispose();

            Log.Log("Finished.");
        }

        private void HandleNewClients()
        {
            ClientAuthConnection c = null;
            while ((c = clientAccepter.GetVarifiedClient()) != null)
            {
                /*
                GameInstance instFound = instances.FirstOrDefault((inst) =>
                {

                });
                foreach (GameInstance inst in instances)
                {
                    var plrFound = inst.GetPlayers().FirstOrDefault((p) => (p.Info.Name.CompareTo(c.VerifyUsername) == 0));
                    if (plrFound != null)
                    {

                    }
                }


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
                 * */
            }
        }
    }
}
