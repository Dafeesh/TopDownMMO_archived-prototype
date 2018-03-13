using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Net;

using Extant;
using Extant.Networking;
using SharedComponents.Global;

public class InstanceServerConnection : MonoComponent
{
    //
    public static InstanceServerConnection Main = null;
    //

    public event Delegate_ConnectionStateChange StateChanged;

    [SerializeField]
    int packetCount = 0;

    IPEndPoint remoteEndPoint = null;
    NetConnection connection = null;
    IPacketDistributor connection_distribution;
    string username = null;
    int? passwordToken = null;

    ConnectionState _state = ConnectionState.Null;

    void Awake()
    {
        Main = this;

        this.connection_distribution = new ClientToInstancePackets.Distribution()
        {
            Default = OnReceive_Default,

            out_Error_c = OnReceive_Error_c,

            out_Verify_Result_c = OnReceive_Verify_Result_c,

            out_Map_Reset_c = OnReceive_Map_Reset_c,
            out_Map_TerrainBlock_c = OnReceive_Map_TerrainBlock_c,

            out_Player_SetControl_c = OnReceive_Player_SetControl_c,

            out_Character_Add_c = OnReceive_Character_Add_c,
            out_Character_Remove_c = OnReceive_Character_Remove_c,
            out_Character_Position_c = OnReceive_Character_Position_c,
            out_Character_Movement_c = OnReceive_Character_Movement_c,
            out_Character_UpdateStats_c = OnReceive_Character_UpdateStats_c
        };

        this.Log.MessageLogged += Debug.Log;
    }

    void Start()
    {
        State = ConnectionState.NoConnection;
    }

    void OnDestroy()
    {
        if (connection != null)
            connection.Dispose();
        StateChanged = null;
    }

    void Update()
    {
        switch (State)
        {
            case (ConnectionState.NoConnection):
                {

                }
                break;
            case (ConnectionState.Connecting):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        Log.Log("InstConnection failed to connect.");
                        State = ConnectionState.NoConnection;
                        break;
                    }

                    if (connection.State == NetConnection.NetworkState.Active)
                    {
                        connection.SendPacket(new ClientToInstancePackets.Verify_Details_i(GameVersion.Build, username, passwordToken.Value));
                        State = ConnectionState.Authorizing;
                    }
                }
                break;
            case (ConnectionState.Authorizing):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        Log.Log("InstConnection disconnected while authorizing.");
                        State = ConnectionState.NoConnection;
                        break;
                    }

                    bool receivedPacket = connection.DistributePacket(connection_distribution);
                    if (receivedPacket == true && State == ConnectionState.Authorizing) //If no action was taken, then wrong packet.
                    {
                        Log.Log("Received wrong packet while authorizing.");
                        CloseConnection();
                    }
                }
                break;
            case (ConnectionState.Connected):
                {
                    if (connection.State != NetConnection.NetworkState.Active)
                    {
                        Log.Log("Disconnected from server!");
                        State = ConnectionState.NoConnection;
                    }
                    else
                    {
                        while (connection.DistributePacket(connection_distribution) == true)
                        {
                            packetCount++;
                        }
                    }
                }
                break;
        }
    }

    public void CloseConnection()
    {
        if (connection != null)
        {
            connection.Dispose();
            connection = null;
            packetCount = 0;

            Log.Log("Cleared connection.");
            State = ConnectionState.NoConnection;
        }
    }

    public void SetTarget(IPEndPoint endPoint, string username, int password)
    {
        if (State != ConnectionState.NoConnection)
        {
            CloseConnection();
        }

        this.remoteEndPoint = endPoint;
        this.username = username;
        this.passwordToken = password;

        connection = new NetConnection(remoteEndPoint);
        //connection.Log.MessageLogged += Debug.Log;
        connection.Start();
        State = ConnectionState.Connecting;

        Log.Log("Set target to " + endPoint.Address.ToString() + ":" + endPoint.Port);
    }

    #region OnReceive

    private void OnReceive_Default(Packet p)
    {
        Debug.LogError("Unhandled packet: " + (ClientToInstancePackets.PacketType)p.Type);
    }

    private void OnReceive_Error_c(ClientToInstancePackets.Error_c p)
    {
        Debug.LogError("ERROR from InstanceServer: " + p.error.ToString());
    }

    private void OnReceive_Verify_Result_c(ClientToInstancePackets.Verify_Result_c p)
    {
        if (p.returnCode == ClientToInstancePackets.Verify_Result_c.VerifyReturnCode.Success)
        {
            Log.Log("Successfully connected!");
            State = ConnectionState.Connected;
        }
        else
        {
            Log.Log("Denied connection: " + p.returnCode.ToString());
        }
    }

    private void OnReceive_Player_SetControl_c(ClientToInstancePackets.Player_SetControl_c p)
    {
        //plrController.SetCharacterControlled(charListController.GetControllerFromId(pp.id));

        Log.Log("Got: set control of char: " + p.charId);
    }

    private void OnReceive_Map_Reset_c(ClientToInstancePackets.Map_Reset_c p)
    {
        //mapController.ResetMap(pp.newNumBlocksX, pp.newNumBlocksY);

        Log.Log("Got: reset Map.");
    }

    private void OnReceive_Map_TerrainBlock_c(ClientToInstancePackets.Map_TerrainBlock_c p)
    {
        //mapController.SetTerrainBlock(pp.blockX, pp.blockY, pp.heightMap);

        Log.Log("Got: terrain block.");
    }

    private void OnReceive_Character_Add_c(ClientToInstancePackets.Character_Add_c p)
    {
        //charListController.AddCharacter(pp.charId, pp.charType, pp.modelNumber);

        Log.Log("Add Character: " + p.charId);
    }

    private void OnReceive_Character_Position_c(ClientToInstancePackets.Character_Position_c p)
    {
        //charListController.SetCharacter_Position(pp.charId, pp.newx, pp.newy);

        Log.Log("Character position: " + p.charId);
    }

    private void OnReceive_Character_Movement_c(ClientToInstancePackets.Character_Movement_c p)
    {
        //charListController.SetCharacter_MovePoint(pp.charId, pp.movePoint);

        Log.Log("Character movement: " + p.charId + "- " + p.movePoint.start.ToString() + "->" + p.movePoint.end.ToString());
    }

    private void OnReceive_Character_Remove_c(ClientToInstancePackets.Character_Remove_c p)
    {
        //charListController.RemoveCharacter(pp.charId);

        Log.Log("Remove Character: " + p.charId);
    }

    private void OnReceive_Character_UpdateStats_c(ClientToInstancePackets.Character_UpdateStats_c p)
    {
        //charListController.SetCharacter_Stats(pp.charId, pp.stats);

        Log.Log("Character stats: " + p.charId);
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
            }
        }
    }
}

/*
Packet p = null;
while ((p = wsConnection.GetPacket()) != null)
{
    switch ((ClientToWorldPackets.PacketType)p.Type)
    {
        case (ClientToWorldPackets.PacketType.Error_c):
            {
                ClientToWorldPackets.Error_c pp = p as ClientToWorldPackets.Error_c;

                Debug.LogError("ERROR from WorldServer: " + pp.error.ToString());
                Log.Log("ERROR: " + pp.error.ToString());
            }
            break;

        case (ClientToWorldPackets.PacketType.Player_Info_c):
            {
                ClientToWorldPackets.Player_Info_c pp = p as ClientToWorldPackets.Player_Info_c;

                playerInfo.Name = pp.username;
                playerInfo.Level = pp.level;

                uiController.SetPlayerName(pp.username);
                uiController.SetPlayerLevel(pp.level);

                Log.Log("Got player info: " + playerInfo.Name + "/" + playerInfo.Level);
            }
            break;

        case (ClientToWorldPackets.PacketType.Player_SetControl_c):
            {
                ClientToWorldPackets.Player_SetControl_c pp = p as ClientToWorldPackets.Player_SetControl_c;

                plrController.SetCharacterControlled(charListController.GetControllerFromId(pp.id));

                Log.Log("Set control of char: " + pp.id);
            }
            break;

        case (ClientToWorldPackets.PacketType.Map_Reset_c):
            {
                ClientToWorldPackets.Map_Reset_c pp = p as ClientToWorldPackets.Map_Reset_c;

                mapController.ResetMap(pp.newNumBlocksX, pp.newNumBlocksY);

                Log.Log("Reset Map.");
            }
            break;

        case (ClientToWorldPackets.PacketType.Map_TerrainBlock_c):
            {
                ClientToWorldPackets.Map_TerrainBlock_c pp = p as ClientToWorldPackets.Map_TerrainBlock_c;

                mapController.SetTerrainBlock(pp.blockX, pp.blockY, pp.heightMap);

                Log.Log("Terrain Block.");
            }
            break;

        case (ClientToWorldPackets.PacketType.Character_Add_c):
            {
                ClientToWorldPackets.Character_Add_c pp = p as ClientToWorldPackets.Character_Add_c;

                //FIX
                //charListController.AddCharacter(pp.charId, pp.charType, pp.modelNumber);

                Log.Log("Add Character: " + pp.charId);
            }
            break;

        case (ClientToWorldPackets.PacketType.Character_Position_c):
            {
                ClientToWorldPackets.Character_Position_c pp = p as ClientToWorldPackets.Character_Position_c;

                charListController.SetCharacter_Position(pp.charId, pp.newx, pp.newy);

                Log.Log("Character position: " + pp.charId);
            }
            break;

        case (ClientToWorldPackets.PacketType.Character_Movement_c):
            {
                ClientToWorldPackets.Character_Movement_c pp = p as ClientToWorldPackets.Character_Movement_c;

                charListController.SetCharacter_MovePoint(pp.charId, pp.movePoint);

                Log.Log("Character movement: " + pp.charId + "- " + pp.movePoint.start.ToString() + "->" + pp.movePoint.end.ToString());
            }
            break;

        case (ClientToWorldPackets.PacketType.Character_Remove_c):
            {
                ClientToWorldPackets.Character_Remove_c pp = p as ClientToWorldPackets.Character_Remove_c;

                charListController.RemoveCharacter(pp.charId);

                Log.Log("Remove Character: " + pp.charId);
            }
            break;

        case (ClientToWorldPackets.PacketType.Character_UpdateStats_c):
            {
                ClientToWorldPackets.Character_UpdateStats_c pp = p as ClientToWorldPackets.Character_UpdateStats_c;

                charListController.SetCharacter_Stats(pp.charId, pp.stats);

                Log.Log("Character stats: " + pp.charId);
            }
            break;

        default:
            {
                Log.Log("Got unknown packet: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
            }
            break;
    }
}
 */