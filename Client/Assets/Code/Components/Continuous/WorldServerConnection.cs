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
    int CONNECT_TIMEOUT = 5000;
    [SerializeField]
    int RECEIVE_TIMEOUT = 30000;

    [SerializeField]
    int packetCount = 0;

    [SerializeField]
    DebugInterfaceController debugInterface = null;

    IPEndPoint remoteEndPoint = null;
    NetConnection connection = null;
    string username = null;
    int password = -1;

    DebugLogger log = new DebugLogger();

    ConnectionState state = ConnectionState.Connected;
    Queue<Packet> packets = new Queue<Packet>();

    void Start()
    {
        log.AnyLogged += Debug.Log;

        if (debugInterface == null)
            Debug.LogError("WSCon not given a reference to DebugInterface.");

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
                                    Application.LoadLevel(SceneList.GameStart);
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
                    if (connection.State != NetConnection.NetworkState.Connected)
                    {
                        SetState(ConnectionState.None);
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
                        if (Application.loadedLevelName != "_MainMenu")
                        {
                            Application.LoadLevel("_MainMenu");
                        }
                    }
                    break;
                case (ConnectionState.Connected):
                    {

                    }
                    break;
            }
            debugInterface.Text_WSConnextion = state.ToString();
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
