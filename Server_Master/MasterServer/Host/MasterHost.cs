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
            for (int i = clients.Count - 1; i >= 0; i--)
            {
                ClientLink cl = clients[i];

                cl.BroadcastActions();

                if (!cl.IsConnected && cl.State == ClientLink.ClientState.CharSelect)
                {
                    clients.Remove(cl);

                    if (ClientRemoved != null)
                        ClientRemoved(cl);

                    Log.Log("Client removed: " + cl.AccountInfo.Name);
                }
            }
        }

        private void GetNewClients()
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
                    if (ClientUpdated != null)
                        ClientUpdated(newLink);

                    newLink.Send_CharacterList();
                    Log.Log("New client: " + newLink.AccountInfo.Name);
                }
                else //Else, it is an already existing client.
                {
                    if (!foundClient.IsConnected)
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
                            sender.Send_WorldServerInfo(serverHub.GetWorldServer(selectedWorld));
                            break;

                        case (ServerHub.ServerState.WorldOffline):
                            Log.Log("Client could not connect because world is offline: " + selectedWorld);
                            sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InternalError);
                            break;

                        default:
                            sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InvalidOperation);
                            break;
                    }
                }
                else
                {
                    sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InvalidOperation);
                }
            }
            else
            {
                Log.Log("[ERROR] Client selected character while not in the proper state.");
                sender.Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode.InvalidOperation);
            }
        }

        #endregion ReceivedActions
    }
}
