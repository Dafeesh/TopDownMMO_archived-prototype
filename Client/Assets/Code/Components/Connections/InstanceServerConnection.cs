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
    string username = null;
    int? passwordToken = null;

    ConnectionState _state = ConnectionState.Null;
    Queue<Packet> packets = new Queue<Packet>();

    void Awake()
    {
        Main = this;
        Log.MessageLogged += Debug.Log;
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
                    }
                    else if (connection.State == NetConnection.NetworkState.Connected)
                    {
                        connection.SendPacket(new ClientToWorldPackets.Verify_Details_g(GameVersion.Build, username, passwordToken.Value));
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
                    }
                    else
                    {
                        Packet p = connection.GetPacket();
                        if (p != null)
                        {
                            if ((ClientToWorldPackets.PacketType)p.Type == ClientToWorldPackets.PacketType.Verify_Result_c)
                            {
                                ClientToWorldPackets.Verify_Result_c r = (ClientToWorldPackets.Verify_Result_c)p;
                                if (r.returnCode == ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.Success)
                                {
                                    Log.Log("Successfully connected!");
                                    State = ConnectionState.Connected;
                                }
                                else
                                {
                                    Log.Log("Denied connection: " + r.returnCode.ToString());
                                }
                            }
                            else
                            {
                                Log.Log("Received wrong packet when authorizing: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
                                CloseConnection();
                            }
                        }
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
                        Packet p = null;
                        while ((p = connection.GetPacket()) != null)
                        {
                            packetCount++;
                            packets.Enqueue(p);
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

        connection = new NetConnection(ClientToWorldPackets.ReadBuffer, remoteEndPoint, CONNECT_TIMEOUT);
        //connection.Log.MessageLogged += Debug.Log;
        connection.Start();
        State = ConnectionState.Connecting;

        Log.Log("Set target to " + endPoint.Address.ToString() + ":" + endPoint.Port);
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
            }
        }
    }
}
