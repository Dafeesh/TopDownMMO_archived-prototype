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

    public event Delegate_AddCharacterListItem Received_AddCharacterListItem;
    public delegate void Delegate_AddCharacterListItem(string name, CharacterLayout layout, int level);

    [SerializeField]
    int packetCount = 0;

    ConnectionState _state;
    NetConnection connection = null;
    IPEndPoint connection_target = null;
    string connection_username = null;
    string connection_password = null;

    void Awake()
    {
        Main = this;
        Log.MessageLogged += Debug.Log;
    }

    void Start()
    {
        State = ConnectionState.NoConnection;

        Log.Log("Start.");
    }

    void OnDestroy()
    {
        //Kill connection
        if (connection != null)
            connection.Dispose();

        //Flush events
        StateChanged = null;
        Received_AddCharacterListItem = null;

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
                    else if (connection.State == NetConnection.NetworkState.Connected)
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
                        Packet p = null;
                        if ((p = connection.GetPacket()) != null)
                        {
                            if (p is ClientToMasterPackets.AccountAuthorize_Response_c)
                            {
                                var pp = p as ClientToMasterPackets.AccountAuthorize_Response_c;

                                switch (pp.Response)
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
                                            Log.Log("Unsupported response from Master: " + pp.Response.ToString());
                                            CloseConnection();
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                Log.Log("Received wrong packet while verifying.");
                                CloseConnection();
                            }
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
                        Packet p = null;
                        while ((p = connection.GetPacket()) != null)
                        {
                            switch ((ClientToMasterPackets.PacketType)p.Type)
                            {
                                case (ClientToMasterPackets.PacketType.Menu_CharacterListItem_c):
                                    {
                                        var pp = p as ClientToMasterPackets.Menu_CharacterListItem_c;

                                        if (Received_AddCharacterListItem != null)
                                            Received_AddCharacterListItem(pp.Name, pp.Layout, pp.Level);
                                    }
                                    break;
                            }
                        }
                    }
                }
                break;
        }

    }

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

    public void CloseConnection()
    {
        if (connection != null)
        {
            connection.Dispose();
            connection = null;
            State = ConnectionState.NoConnection;
        }
    }

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

                if (_state != ConnectionState.NoConnection)
                {
                    if (Application.loadedLevelName != ResourceList.Scenes.CharSelect)
                        Application.LoadLevel(ResourceList.Scenes.CharSelect);
                }
                else if (_state == ConnectionState.NoConnection)
                {
                    if (Application.loadedLevelName != ResourceList.Scenes.LoginPage)
                        Application.LoadLevel(ResourceList.Scenes.LoginPage);
                }
            }
        }
    }

    public void SetTarget(IPEndPoint target, string username, string password)
    {
        connection_target = target;
        connection_username = username;
        connection_password = password;

        NetConnection con = new NetConnection(ClientToMasterPackets.ReadBuffer, target, 5000);
        con.Start();
        connection = con;

        State = ConnectionState.Connecting;
    }
}
