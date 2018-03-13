using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SharedComponents.Server;
using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

using Extant;
using Extant.Networking;

using MasterServer.Game;

namespace MasterServer.Links
{
    public class ClientLink : IDisposable , ILogging
    {
        private AccountInfo accountInfo;
        private NetConnection connection;
        private IPacketDistributor connection_distribution;
        private ActionDispersion actionDispersion;

        private PlayerCharacter activeCharacter = null;

        private bool _disposed = false;
        private DebugLogger _log;

        public class ActionDispersion : IDisposable
        {
            public Delegate_OnAction_CharacterListItem_Select OnAction_CharacterListItem_Select = null;

            public void Dispose()
            {
                OnAction_CharacterListItem_Select = null;
            }
        }

        public ClientLink(ActionDispersion actionDispersion, AccountInfo accountInfo, NetConnection clientConnection)
        {
            this.connection_distribution = new ClientToMasterPackets.Distribution()
            {
                out_Menu_CharacterListItem_Select_m = OnReceive_Menu_CharacterListItem_Select_m
            };

            this.actionDispersion = actionDispersion;
            this.accountInfo = accountInfo;
            this.connection = clientConnection;

            this._log = new DebugLogger("CLink:" + accountInfo.Name);
            this._log.MessageLogged += Console.WriteLine;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                connection_distribution.Dispose();

                connection.Dispose();
            }
        }

        public void BroadcastActions()
        {
            //This will have packets be sent to the functions in OnReceivePackets region.
            //Each function will broadcast to the given actionDispersion object.
            while (connection.DistributePacket(connection_distribution) == true)
            { }
        }

        public override string ToString()
        {
            return accountInfo.Name;
        }

        public void SetConnection(NetConnection con)
        {
            if (this.HasConnection)
            {
                connection.Dispose();
            }

            connection = con;
        }

        public void SetActiveCharacter(PlayerCharacter ch)
        {
            activeCharacter = ch;
        }

        public bool HasConnection
        {
            get
            {
                return (connection.State == NetConnection.NetworkState.Active);
            }
        }

        public AccountInfo AccountInfo
        {
            get
            {
                return accountInfo;
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }
        }

        #region SendPackets

        public void Send_CharacterList()
        {
            foreach (PlayerCharacterInfo charInfo in accountInfo.Characters)
            {
                connection.SendPacket(new ClientToMasterPackets.Menu_CharacterListItem_c(
                    charInfo.Name,
                    charInfo.VisualLayout,
                    charInfo.Level));
            }
        }

        #endregion SendPackets

        #region OnReceivePackets

        public delegate void Delegate_OnAction_CharacterListItem_Select(ClientLink client, string selectedCharName);
        private void OnReceive_Menu_CharacterListItem_Select_m(ClientToMasterPackets.Menu_CharacterListItem_Select_m packet)
        {
            Console.WriteLine("Received selection: " + packet.CharName);
            if (actionDispersion.OnAction_CharacterListItem_Select != null)
                actionDispersion.OnAction_CharacterListItem_Select(this, packet.CharName);
        }

        #endregion OnReceivePackets
    }
}
