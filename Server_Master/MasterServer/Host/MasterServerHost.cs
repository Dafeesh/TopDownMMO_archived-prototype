using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using MasterServer.Links;
using MasterServer.Game;

using Extant;
using Extant.Networking;

namespace MasterServer.Host
{
    public class MasterServerHost : ThreadRun
    {
        public event Delegate_ClientUpdate ClientAdded;
        public event Delegate_ClientUpdate ClientUpdated;
        public delegate void Delegate_ClientUpdate(ClientLink cLink);

        private GameWorld gameWorld;
        private List<ClientLink> clients = new List<ClientLink>();
        private ClientLink.ActionDispersion clients_ActionDispersion;

        private ClientAcceptor clientAcceptor;

        public MasterServerHost(ClientAcceptor clientAcceptor, InstanceServerHub serverHub)
            : base("MasterServer")
        {
            this.clientAcceptor = clientAcceptor;
            this.clients_ActionDispersion = new ClientLink.ActionDispersion()
            {
                OnAction_CharacterListItem_Select = OnAction_CharListItem_Select
            };

            this.gameWorld = new GameWorld(serverHub);

            Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            clients_ActionDispersion.Dispose();
            clientAcceptor.Start();

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            gameWorld.PollServers();

            HandleClients();
            HandleNewClients();
        }

        protected override void Finish(bool success)
        {
            clientAcceptor.Stop("MasterServerHost finished.");
            gameWorld.Dispose();

            foreach (var c in clients)
            {
                c.Dispose();
            }

            Log.Log("Finish.");
        }

        private void HandleClients()
        {
            foreach (ClientLink cl in clients)
            {
                cl.BroadcastActions();
            }
        }

        private void HandleNewClients()
        {
            ClientAcceptor.AuthorizedLoginAttempt newAuthorizedClient = null;
            while ((newAuthorizedClient = clientAcceptor.GetAuthorizedLoginAttempt()) != null)
            {
                //Get client if already exists.
                ClientLink foundClient = clients.FirstOrDefault((cl) =>
                    {
                        return cl.AccountInfo.Name.CompareTo(newAuthorizedClient.Info.Name) == 0;
                    });

                //If not found, then it is a newly connecting client.
                if (foundClient == null)
                {
                    ClientLink newLink = new ClientLink(clients_ActionDispersion, newAuthorizedClient.Info, newAuthorizedClient.Connection);

                    clients.Add(newLink);
                    if (ClientAdded != null)
                        ClientAdded(newLink);

                    newLink.Send_CharacterList();
                    Log.Log("New client: " + newLink.AccountInfo.Name);
                }
                else //Else, it is an already existing client.
                {
                    if (!foundClient.HasConnection)
                    {
                        foundClient.SetConnection(newAuthorizedClient.Connection);
                        if (ClientUpdated != null)
                            ClientUpdated(foundClient);

                        foundClient.Send_CharacterList();
                        Log.Log("Client reconnected: " + newAuthorizedClient.Info.Name);
                    }
                    else
                    {
                        newAuthorizedClient.Connection.Dispose();

                        Log.Log("Client already connected: " + newAuthorizedClient.Info.Name);
                    }
                }
            }
        }

        #region ReceivedActions

        void OnAction_CharListItem_Select(ClientLink sender, string selectedCharacter)
        {
            Console.WriteLine("CharListSelect: " + selectedCharacter);
        }

        #endregion ReceivedActions
    }
}
