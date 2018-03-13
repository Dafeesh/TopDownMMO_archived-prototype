using UnityEngine;
using System.Collections;
using System.Net;

using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

using Extant;
using Extant.Networking;

public class MasterServerConnection : MonoComponent
{
    //
    public static MasterServerConnection Main = null;
    //

    public event Delegate_ConnectionStateChange StateChanged;

    private CharListMenuController CharListMenu = null;

    [SerializeField]
    int packetCount = 0;

    ConnectionState _state;
    NetConnection connection = null;
    IPacketDistributor connection_distribute;
    string connection_username = null;
    string connection_password = null;

    void Awake()
    {
        Main = this;

        this.connection_distribute = new ClientToMasterPackets.Distribution()
        {
            out_ErrorCode_c = OnReceive_ErrorCode_c,

            out_AccountAuthorize_Response_c = OnReceive_AccountAuthorize_Response_c,

            out_Menu_CharacterListItem_c = OnReceive_Menu_CharacterListItem_c
        };
        this.Log.MessageLogged += Debug.Log;
    }

    void Start()
    {
        State = ConnectionState.NoConnection;

        Log.Log("Start.");
    }

    public void CloseConnection()
    {
        if (connection != null)
        {
            connection.Dispose();
            connection = null;
            State = ConnectionState.NoConnection;
        }
    }

    bool CachedComponent_CharSelect()
    {
        if (CharListMenu == null)
        {
            CharListMenu = CharListMenuController.Main;
            return (CharListMenu != null);
        }
        else
            return true;
    }

    void OnDestroy()
    {
        CloseConnection();

        //Flush events
        StateChanged = null;

        Log.Log("Destroyed.");
    }

    void Update()
    {
        switch (State)
        {
            case (ConnectionState.Connecting):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        Log.Log("Could not connect.");
                        CloseConnection();
                    }
                    else if (connection.State == NetConnection.NetworkState.Active)
                    {
                        connection.SendPacket(new ClientToMasterPackets.AccountAuthorize_Attempt_m(GameVersion.Build, connection_username, connection_password));
                        State = ConnectionState.Authorizing;
                    }
                }
                break;

            case (ConnectionState.Authorizing):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        Log.Log("Disconnected while authenticating.");
                        CloseConnection();
                    }
                    else
                    {
                        bool receivedPacket = connection.DistributePacket(connection_distribute);

                        //If no action has been taken. (Connected = success, NoConnection = failed)
                        if (receivedPacket && State == ConnectionState.Authorizing)
                        {
                            Log.Log("Received wrong packet while authorizing.");
                            CloseConnection();
                        }
                    }
                }
                break;

            case (ConnectionState.Connected):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        Log.Log("Disconnected.");
                        CloseConnection();
                    }
                    else
                    {
                        //Receive all packets available.
                        while (connection.DistributePacket(connection_distribute) == true)
                        {
                            packetCount++;
                        }
                    }
                }
                break;
        }

    }

    #region OnAction

    public void OnAction_SelectCharacter(string name)
    {
        if (State == ConnectionState.Connected)
        {
            Log.Log("Sent selection: " + name);
            connection.SendPacket(new ClientToMasterPackets.Menu_CharacterListItem_Select_m(name));
        }
        else
        {
            Log.Log("Invalid action in current state: OnAction_SelectCharacter -> " + State.ToString());
        }
    }

    #endregion OnAction

    #region OnReceive

    private void OnReceive_ErrorCode_c(ClientToMasterPackets.ErrorCode_c p)
    {
        Log.Log("Received error from server: " + p.error.ToString());
        CloseConnection();
    }

    private void OnReceive_AccountAuthorize_Response_c(ClientToMasterPackets.AccountAuthorize_Response_c p)
    {
        if (State != ConnectionState.Authorizing)
        {
            Log.Log("Received authorization response while not authorizing.");
            return;
        }

        switch (p.Response)
        {
            case (ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.Success):
                {
                    Log.Log("Successfully connected!");
                    State = ConnectionState.Connected;
                }
                break;

            case (ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidLogin):
                {
                    Log.Log("Invalid login.");
                    CloseConnection();
                }
                break;

            case (ClientToMasterPackets.AccountAuthorize_Response_c.AuthResponse.InvalidBuild):
                {
                    Log.Log("Invalid build.");
                    CloseConnection();
                }
                break;

            default:
                {
                    Log.Log("Unsupported response from Master: " + p.Response.ToString());
                    CloseConnection();
                }
                break;
        }
    }

    private void OnReceive_Menu_CharacterListItem_c(ClientToMasterPackets.Menu_CharacterListItem_c p)
    {
        if (CachedComponent_CharSelect())
        {
            CharListMenu.AddCharacterListItem(p.Name, p.VisualLayout, p.Level);
        }
    }

    #endregion OnReceive

    public ConnectionState State
    {
        get
        {
            return _state;
        }

        private set
        {
            if (value != _state)
            {
                _state = value;

                if (StateChanged != null)
                    StateChanged(_state);

                if (_state == ConnectionState.NoConnection)
                {
                    if (Application.loadedLevelName != ResourceList.Scenes.LoginPage)
                        Application.LoadLevel(ResourceList.Scenes.LoginPage);
                }
                else if (_state == ConnectionState.Connected)
                {
                    if (Application.loadedLevelName != ResourceList.Scenes.CharSelect)
                        Application.LoadLevel(ResourceList.Scenes.CharSelect);
                }
            }
        }
    }

    public void SetTarget(IPEndPoint target, string username, string password)
    {
        connection_username = username;
        connection_password = password;

        NetConnection con = new NetConnection(target);
        con.Start();
        connection = con;

        State = ConnectionState.Connecting;
    }
}
