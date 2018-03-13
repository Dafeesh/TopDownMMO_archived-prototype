using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SharedComponents.Server;
using SharedComponents.Global;
using SharedComponents.Global.Game;

using Extant;
using Extant.Networking;

namespace MasterServer.Links
{
    public class ClientLink : IDisposable, ILogging
    {
        public AccountInfo AccountInfo
        { get; private set; }
        public PlayerCharacterInfo ActiveCharacter
        { get; set; }

        private NetConnection connection;
        private IPacketDistributor connection_distribution;
        private ActionDispersion actionDispersion;

        private ClientState _state = ClientState.CharSelect;
        private bool _disposed = false;
        private DebugLogger _log;

        public class ActionDispersion : IDisposable
        {
            public Delegate_OnAction_CharacterListItem_Select OnAction_CharacterListItem_Select = null;

            public void Dispose()
            {
                Console.WriteLine("Dispose)");
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
            this.AccountInfo = accountInfo;
            this.connection = clientConnection;

            this.Log = new DebugLogger("ClLink:" + accountInfo.Name);
            this.Log.MessageLogged += Console.WriteLine;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

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
            return AccountInfo.Name;
        }

        public void SetConnection(NetConnection con)
        {
            if (this.IsConnected)
            {
                connection.Dispose();
            }

            connection = con;
        }

        public bool IsConnected
        {
            get
            {
                return (connection.State == NetConnection.NetworkState.Active);
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }

            private set
            {
                _log = value;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return _disposed;
            }

            private set
            {
                _disposed = value;
            }
        }

        public ClientState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        #region SendPackets

        public void Send_ErrorCode(ClientToMasterPackets.ErrorCode_c.ErrorCode code)
        {
            connection.SendPacket(new ClientToMasterPackets.ErrorCode_c(code));
        }

        public void Send_CharacterList()
        {
            foreach (PlayerCharacterInfo charInfo in AccountInfo.Characters)
            {
                connection.SendPacket(new ClientToMasterPackets.Menu_CharacterListItem_c(
                    charInfo.Name,
                    charInfo.VisualLayout,
                    charInfo.Level));
            }
        }

        public void Send_WorldServerInfo(WorldServer ws)
        {
            connection.SendPacket(new ClientToMasterPackets.Connection_WorldServerInfo_c(
                ws.BroadcastEndPoint,
                ActiveCharacter.Name
                ));
        }

        #endregion SendPackets

        #region OnReceivePackets

        public delegate void Delegate_OnAction_CharacterListItem_Select(ClientLink client, string selectedCharName, int selectedWorld);
        private void OnReceive_Menu_CharacterListItem_Select_m(ClientToMasterPackets.Menu_CharacterListItem_Select_m packet)
        {
            if (actionDispersion.OnAction_CharacterListItem_Select != null)
                actionDispersion.OnAction_CharacterListItem_Select(this, packet.CharName, packet.WorldNumber);
        }

        #endregion OnReceivePackets

        public enum ClientState
        {
            CharSelect,
            InWorld
        }
    }
}
