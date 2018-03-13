using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

using Extant;
using Extant.Networking;
using Extant.GameServerShared;
using Extant.GlobalShared;

public class GameServerConnection : MonoBehaviour
{
    private NetConnection connection = null;
    private IPEndPoint connection_endPoint = null;
    private string username;
    private int password;

    private bool hasStarted = false;
    
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (connection == null)
        {
            if (connection_endPoint != null)
                Connect();
        }
        else
        {
            if (connection.State == NetConnection.NetworkState.Failed)
            {
                DebugLogger.GlobalDebug.LogNetworking("Disconnected from game server!");
                StopAndCleanup();
            }
        }
    }

    void OnApplicationQuit()
    {
        if (connection != null)
        {
            connection.Stop();
        }
    }

    private void Connect()
    {
        connection = new NetConnection(connection_endPoint, 5000, 10000);
        connection.Start();

        DebugLogger.GlobalDebug.LogNetworking("Attempting to connect to " + connection_endPoint.AddressFamily.ToString() + "/" + connection_endPoint.Port.ToString());
        while (connection.State == NetConnection.NetworkState.Connecting || connection.State == NetConnection.NetworkState.Waiting)
        { }

        if (connection.State == NetConnection.NetworkState.Failed)
        {
            DebugLogger.GlobalDebug.LogNetworking("Could not connect.");
            StopAndCleanup();
            return;
        }

        DebugLogger.GlobalDebug.LogNetworking("Authorizing...");
        connection.SendPacket(new GameServerPackets.Verify_Details_g(GameVersion.Build, username, password));

        Packet returnPacket = null;
        while ((connection.State == NetConnection.NetworkState.Connected) && 
               (returnPacket = connection.GetPacket()) == null)
        { }

        //Disconnected before it could get a packet from server
        if (returnPacket == null)
        {
            DebugLogger.GlobalDebug.LogCatch("Did not receive an authentication response from server.");
            StopAndCleanup();
            return;
        }

        //Received the wrong packet from the server
        if (!(returnPacket is GameServerPackets.Verify_Result_c))
        {
            DebugLogger.GlobalDebug.LogCatch("Received wrong packet from server.");
            StopAndCleanup();
            return;
        }

        //Check if Verify code is success
        if (!((returnPacket as GameServerPackets.Verify_Result_c).errorCode == GameServerPackets.Verify_Result_c.VerifyReturnCode.Success))
        {
            DebugLogger.GlobalDebug.LogCatch("Received wrong packet from server.");
            StopAndCleanup();
            return;
        }

        DebugLogger.GlobalDebug.LogThisGame("Verified! Loading game...");
    }

    public void SetConnectTarget(string ip, int port, string user, int pass)
    {
        connection_endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        username = user;
        password = pass;
    }

    public void StopAndCleanup()
    {
        if (connection != null)
        {
            connection.Stop();
            connection = null;
        }
        connection_endPoint = null;
    }

    public Packet GetPacket()
    {
        if (connection != null)
        {
            return connection.GetPacket();
        }
        else
        {
            return null;
        }
    }

    public bool IsConnected
    {
        get
        {
            return (connection == null) ? (false) : (connection.State == NetConnection.NetworkState.Connected);
        }
    }
}