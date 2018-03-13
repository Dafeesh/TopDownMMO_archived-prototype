using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Extant;
using Extant.Networking;

using SharedComponents.Global;

using MasterServer.Links;
using MasterServer.Database;

namespace MasterServer.Host
{
    public class MasterHost : ThreadRun
    {
        //<events>
        public event Delegate_ClientUpdate ClientUpdated;
        public event Delegate_ClientUpdate ClientRemoved;
        public delegate void Delegate_ClientUpdate(ClientLink cLink);
        //</events>

        private ServerHub serverHub;
        private List<ClientLink> clients = new List<ClientLink>();
        private ClientLink.ActionDispersion clients_ActionDispersion;

        private ClientAcceptor clientAcceptor;

        public MasterHost(ClientAcceptor clientAcceptor, ServerHub serverHub)
            : base("MasterServer")
        {
            this.clientAcceptor = clientAcceptor;
            this.clients_ActionDispersion = new ClientLink.ActionDispersion()
            {
                OnAction_CharacterListItem_Select = OnAction_CharListItem_Select
            };

            this.serverHub = serverHub;

            Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            clientAcceptor.Start();

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            HandleClients();
            GetNewClients();
        }

        protected override void Finish(bool success)
        {
            clientAcceptor.Dispose();
            serverHub.Dispose();

            foreach (var c in clients)
            {
                c.Dispose();
            }

            Log.Log("Finish.");
        }

        private void HandleClients()
        {
            ClientLink client;
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                client = clients[i];

                client.BroadcastActions();

                if (!client.IsConnected && client.State == ClientLink.ClientState.CharSelect)
                {
                    RemoveExistingClient(client);
                    clients.Remove(client);

                    if (ClientRemoved != null)
                        ClientRemoved(client);

                    Log.Log("Client removed: " + client.AccountInfo.Name);
                }
            }
        }

        private void GetNewClients()
        {
            ClientAcceptor.AuthorizedLoginAttempt newAuthorizedClient = null;
            while ((newAuthorizedClient = clientAcceptor.GetAuthorizedLoginAttempt()) != null)
            {
                //See if client already exists.
                ClientLink foundClient = clients.FirstOrDefault((cl) =>
                    {
                        return cl.AccountInfo.Name.CompareTo(newAuthorizedClient.Info.Name) == 0;
                    });

                //If found, then it is an already existing client on server.
                if (foundClient != null)
                {
                    if (foundClient.IsConnected)
                    {
                        newAuthorizedClient.Connection.Dispose();
                        Log.Log("Client denied login; already connected: " + newAuthorizedClient.Info.Name);
                    }
                    else
                    {
                        foundClient.SetConnection(newAuthorizedClient.Connection);
                        if (ClientUpdated != null)
                            ClientUpdated(foundClient);

                        foundClient.Send_CharacterList();
                        Log.Log("Client reconnected: " + newAuthorizedClient.Info.Name);
                    }
                }
                else //If not found, then it is a newly connecting client.
                {
                    ClientLink newLink = new ClientLink(clients_ActionDispersion, newAuthorizedClient.Info, newAuthorizedClient.Connection);
                    newLink.Send_CharacterList();

                    AddNewClient(newLink);
                    Log.Log("New client: " + newLink.AccountInfo.Name);
                }
            }
        }

        void AddNewClient(ClientLink client)
        {
            clients.Add(client);
            if (ClientUpdated != null)
                ClientUpdated(client);
        }

        void RemoveExistingClient(ClientLink client)
        {
            clients.Remove(client);
            if (ClientRemoved != null)
                ClientRemoved(client);
        }

        #region ReceivedActions

        void OnAction_CharListItem_Select(ClientLink sender, string selectedCharacter, int selectedWorld)
        {
            if (sender.State == ClientLink.ClientState.CharSelect)
            {
                var foundChar = (sender.AccountInfo.Characters.FirstOrDefault((c) =>
                {
                    return c.Name == selectedCharacter;
                }));

                if (foundChar != null)
                {
                    sender.ActiveCharacter = foundChar;

                    var serverStatus = serverHub.GetWorldServerStatus(selectedWorld);
                    switch (serverStatus)
                    {
                        case (ServerHub.ServerState.Online):
                            Log.Log("Client entering world: " + selectedCharacter + "/" + selectedWorld);
                            Log.LogError("TODO: MasterHost handle character login.");
                            sender.Send_WorldServerInfo(serverHub.GetWorldServer(selectedWorld));
                            break;

                        default: // (ServerHub.ServerState.WorldOffline):
                            Log.Log("Client could not connect because world is offline: " + selectedWorld);
                            sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InternalError);
                            break;
                    }
                }
                else
                {
                    Log.LogWarning("Invalid selected character: " + sender.AccountInfo.Name + " -> " + selectedCharacter);
                    sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InvalidOperation);
                }
            }
            else
            {
                Log.LogWarning("Client selected character while not in the proper state.");
                sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InvalidOperation);
            }
        }

        #endregion ReceivedActions
    }
}
