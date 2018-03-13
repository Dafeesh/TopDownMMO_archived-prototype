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

namespace MasterServer.Links
{
    public class ClientLink : IDisposable , ILogging
    {
        private AccountInfo accountInfo;
        private NetConnection connection;

        private bool _isInGame = false;
        private bool _disposed = false;
        private DebugLogger _log;

        public ClientLink(AccountInfo accountInfo, NetConnection clientConnection)
        {
            this.accountInfo = accountInfo;
            this.connection = clientConnection;

            this._log = new DebugLogger("CLink:" + accountInfo.Name);
            this._log.MessageLogged += Console.WriteLine;

            Send_CharacterList();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                connection.Stop("ClientLink finished.");
                connection.Dispose();
            }
        }

        public void TriggerReceivedActions()
        {
            Packet p = null;
            while ((p = connection.GetPacket()) != null)
            {
                if (IsInGame)
                {
                    if (false)
                    {

                    }
                    else
                    {
                        Log.Log("Received invalid packet while InGame: " + (ClientToMasterPackets.PacketType)p.Type);
                    }
                }
                else
                {
                    if (p is ClientToMasterPackets.Menu_CharacterListItem_Select_m)
                    {
                        ClientLink.ReceivedActions.Trigger_CharListSelect(this, (p as ClientToMasterPackets.Menu_CharacterListItem_Select_m).CharName);
                    }
                    else
                    {
                        Log.Log("Received invalid packet while not InGame: " + (ClientToMasterPackets.PacketType)p.Type);
                    }
                }
            }
        }

        public override string ToString()
        {
            return accountInfo.Name;
        }

        public void SetConnection(NetConnection con)
        {
            if (this.HasConnection)
            {
                connection.Stop("Client overwrote connection.");
                connection.Dispose();
            }

            connection = con;
            Send_CharacterList();
        }

        public bool IsInGame
        {
            get
            {
                return _isInGame;
            }

            private set
            {
                _isInGame = value;
            }
        }

        public bool HasConnection
        {
            get
            {
                return connection.State == NetConnection.NetworkState.Connected;
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

        private void Send_CharacterList()
        {
            foreach (CharacterInfo charInfo in accountInfo.Characters)
            {
                connection.SendPacket(new ClientToMasterPackets.Menu_CharacterListItem_c(
                    charInfo.Name,
                    charInfo.Layout,
                    charInfo.Level));
            }
        }

        #endregion SendPackets

        public static class ReceivedActions
        {
            public static event Delegate_CharListSelect CharListSelect;
            public delegate void Delegate_CharListSelect(ClientLink sender, string name);
            public static void Trigger_CharListSelect(ClientLink sender, string name)
            {
                if (CharListSelect != null)
                    CharListSelect(sender, name);
            }
        }
    }
}
