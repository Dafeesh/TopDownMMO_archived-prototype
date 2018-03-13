using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using MasterServer.Links;

using Extant;
using Extant.Networking;

namespace MasterServer.Host
{
    public class MasterServerHost : ThreadRun
    {
        public event Delegate_ClientUpdate ClientAdded;
        public event Delegate_ClientUpdate ClientUpdated;
        public delegate void Delegate_ClientUpdate(ClientLink cLink);

        private List<ClientLink> clients = new List<ClientLink>();
        private ClientLink.ActionDispersion clients_ActionDispersion;

        private ClientAcceptor clientAcceptor;
        private InstanceServerLink[] zoneInstanceServers;
        private InstanceServerLink[] instanceServers;

        public MasterServerHost(ClientAcceptor clientAcceptor, InstanceServerLink[] zoneInstServLinks, InstanceServerLink[] instServerLinks)
            : base("MasterServer")
        {
            this.clientAcceptor = clientAcceptor;
            this.zoneInstanceServers = zoneInstServLinks;
            this.instanceServers = instServerLinks;

            #region ActionSubscriptions
            this.clients_ActionDispersion = new ClientLink.ActionDispersion()
            {
                OnAction_CharacterListItem_Select = OnAction_CharListItem_Select
            };
            #endregion ActionSubscriptions

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
            HandleNewClients();
            PollConnections();
            HandleReceivedActions();
        }

        protected override void Finish(bool success)
        {
            clientAcceptor.Stop("MasterServerHost finished.");
            foreach (var c in clients)
            {
                c.Dispose();
            }
            foreach (var i in zoneInstanceServers)
            {
                i.Dispose();
            }

            foreach (var i in instanceServers)
            {
                i.Dispose();
            }

            Log.Log("Finish.");
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
                        newAuthorizedClient.Connection.Stop("Client already connected.");
                        newAuthorizedClient.Connection.Dispose();

                        Log.Log("Client already connected: " + newAuthorizedClient.Info.Name);
                    }
                }
            }
        }

        private void PollConnections()
        {
            foreach (InstanceServerLink z in zoneInstanceServers)
            {
                z.PollConnection();
            }
            foreach (InstanceServerLink i in instanceServers)
            {
                i.PollConnection();
            }
        }

        private void HandleReceivedActions()
        {
            foreach (ClientLink cl in clients)
            {
                cl.BroadcastActions();
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
