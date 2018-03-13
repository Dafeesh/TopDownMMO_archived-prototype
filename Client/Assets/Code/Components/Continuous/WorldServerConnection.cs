using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Net;

using Extant;
using Extant.Networking;
using SharedComponents;

public class WorldServerConnection : MonoBehaviour
{
    [SerializeField]
    private int CONNECT_TIMEOUT;
    [SerializeField]
    private int RECEIVE_TIMEOUT;

    [SerializeField]
    private int packetCount = 0;

    [SerializeField]
    private Text DebugText_State;

    private IPEndPoint remoteEndPoint = null;
    private NetConnection connection = null;
    private string username = null;
    private int password = -1;

    private DebugLogger log = new DebugLogger();

    private ConnectionState state = ConnectionState.Connected;
    private Queue<Packet> packets = new Queue<Packet>();

    void Start()
    {
        log.AnyLogged += Debug.Log;

        SetState(ConnectionState.None);
    }

    void OnApplicationQuit()
    {
        if (connection != null)
            connection.Dispose();
    }

    void Update()
    {
        switch (State)
        {
            case (ConnectionState.None):
                {

                }
                break;
            case (ConnectionState.Connecting):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        log.Log("WSConnection failed to connect.");
                        SetState(ConnectionState.None);
                    }
                    else if (connection.State == NetConnection.NetworkState.Connected)
                    {
                        log.Log("WSConnection connected!");
                        connection.SendPacket(new ClientToWorldPackets.Verify_Details_g(GameVersion.Build, username, password));
                        SetState(ConnectionState.Authorizing);
                    }
                }
                break;
            case (ConnectionState.Authorizing):
                {
                    if (connection.State == NetConnection.NetworkState.Closed)
                    {
                        log.Log("WSConnection disconnected while authorizing.");
                        SetState(ConnectionState.None);
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
                                    log.Log("WSConnection authorized!");
                                    SetState(ConnectionState.Connected);
                                }
                                else
                                {
                                    log.Log("WSConnection was denied connection: " + r.returnCode.ToString());
                                }
                            }
                            else
                            {
                                log.LogError("WSConnection received wrong packet when authorizing: " + ((ClientToWorldPackets.PacketType)p.Type).ToString());
                                ClearConnection();
                            }
                        }
                    }
                }
                break;
            case (ConnectionState.Connected):
                {
                    Packet p = null;
                    while ((p = connection.GetPacket()) != null)
                    {
                        packetCount++;
                        packets.Enqueue(p);
                    }
                }
                break;
        }
    }

    private void SetState(ConnectionState s)
    {
        if (s != state)
        {
            state = s;
            switch (state)
            {
                case (ConnectionState.None):
                    {
                        ClearConnection();
                    }
                    break;
                case (ConnectionState.Connected):
                    {

                    }
                    break;
            }
            DebugText_State.text = state.ToString();
        }
    }

    public void ClearConnection()
    {
        if (connection != null)
        {
            connection.Dispose();
            packetCount = 0;
            packets.Clear();

            SetState(ConnectionState.None);
            //log.Log("WSConnection cleared connection.");
        }
    }

    public void SetTarget(IPEndPoint endPoint, string username, int password)
    {
        if (State != ConnectionState.None)
        {
            ClearConnection();
        }

        this.remoteEndPoint = endPoint;
        this.username = username;
        this.password = password;

        connection = new NetConnection(ClientToWorldPackets.ReadBuffer, remoteEndPoint, CONNECT_TIMEOUT, RECEIVE_TIMEOUT);
        //connection.SubscribeToLogs(Debug.Log);
        connection.Start();
        SetState(ConnectionState.Connecting);
        log.Log("WSConnection set target to " + endPoint.Address.ToString() + ":" + endPoint.Port);
    }

    public void TEST_Login(string username)
    {
        SetTarget(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000), username, 111);
    }

    public Packet GetPacket()
    {
        if (packets.Count > 0)
        {
            return packets.Dequeue();
        }
        else
            return null;
    }

    public ConnectionState State
    {
        get
        {
            return state;
        }
    }
}
