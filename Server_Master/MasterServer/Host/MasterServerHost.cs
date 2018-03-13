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

        List<ClientLink> clients = new List<ClientLink>();

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
            ClientLink.ReceivedActions.CharListSelect += OnAction_CharListSelect;
            #endregion ActionSubscriptions

            Log.MessageLogged += Console.WriteLine;
        }

        protected override void Begin()
        {
            clientAcceptor.Start();

            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            HandleNewClients();
            PollConnections();
            TriggerReceivedActions();
        }

        protected override void Finish(bool success)
        {
            clientAcceptor.Stop("MasterServerHost finished.");
            foreach (var c in clients)
            {
                c.Dispose();
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

                //Check status
                if (foundClient == null)
                {
                    ClientLink newLink = new ClientLink(newAuthorizedClient.Info, newAuthorizedClient.Connection);

                    clients.Add(newLink);
                    if (ClientAdded != null)
                        ClientAdded(newLink);

                    Log.Log("New client: " + newLink.AccountInfo.Name);
                }
                else
                {
                    if (!foundClient.HasConnection)
                    {
                        foundClient.SetConnection(newAuthorizedClient.Connection);
                        if (ClientUpdated != null)
                            ClientUpdated(foundClient);

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

        private void TriggerReceivedActions()
        {
            foreach (ClientLink cl in clients)
            {
                cl.TriggerReceivedActions();
            }
        }

        #region ReceivedActions

        void OnAction_CharListSelect(ClientLink sender, string selectedCharacter)
        {
            Console.WriteLine("CharListSelect: " + selectedCharacter);
        }

        #endregion ReceivedActions

        public InstanceServerLink[] ZoneServerLinks
        {
            get
            {
                return zoneInstanceServers.ToArray();
            }
        }

        public InstanceServerLink[] InstanceServerLinks
        {
            get
            {
                return instanceServers.ToArray();
            }
        }
    }
}
