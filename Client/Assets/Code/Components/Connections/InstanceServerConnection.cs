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

    int CONNECT_TIMEOUT = 5000;

    [SerializeField]
    int packetCount = 0;

    IPEndPoint remoteEndPoint = null;
    NetConnection connection = null;
    IPacketDistributor connection_distribution;
    string username = null;
    int? passwordToken = null;

    ConnectionState _state = ConnectionState.Null;
    Queue<Packet> packets = new Queue<Packet>();

    void Awake()
    {
        this.connection_distribution = new ClientToInstancePackets.Distribution()
        {
            out_Verify_Result_c = OnReceive_Verify_Result_c
        };

        Main = this;
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

                    if (connection.State == NetConnection.NetworkState.Connected)
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
                    if (receivedPacket == true && State == ConnectionState.Authorizing) //If no action was taken. (Closed = failed, Connected = success)
                    {
                        Log.Log("Received wrong packet while authorizing.");
                        CloseConnection();
                    }
                }
                break;
            case (ConnectionState.Connected):
                {
                    if (connection.State != NetConnection.NetworkState.Connected)
                    {
                        Log.Log("Disconnected from server! Error: " + ((connection.UnhandledException == null) ? ("null") : (connection.UnhandledException.ToString())));
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

        connection = new NetConnection(remoteEndPoint, CONNECT_TIMEOUT);
        //connection.Log.MessageLogged += Debug.Log;
        connection.Start();
        State = ConnectionState.Connecting;

        Log.Log("Set target to " + endPoint.Address.ToString() + ":" + endPoint.Port);
    }

    #region OnReceive

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
