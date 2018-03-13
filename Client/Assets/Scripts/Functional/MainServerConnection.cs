using System;
using System.Net;

using Extant;
using Extant.Networking;

using SharedComponents;

public class MainServerConnection : ThreadRun
{
    private static int TIMEOUT_CONNECT = 5000;
    private static int TIMEOUT_RECEIVE = 10000;
    private static int CONNECTATTEMPTS_MAX = 3;

    private String username;
    private String password;
    private Int32 newPassword;

    private NetConnection connection;
    private IPEndPoint connection_endPoint;
    private Int32 connection_connectAttempts = 0;
    private bool connection_isFullyConnected = false;

    public MainServerConnection(IPEndPoint endPoint, string username, string password)
        : base("GameServerConnection")
    {
        connection_endPoint = endPoint;

        this.username = username;
        this.password = password;
    }

    protected override void Begin()
    {
        bool success = ConnectToServer();

        if (!success)
            this.Stop();
    }

    protected override void RunLoop()
    {
        if (connection.State != NetConnection.NetworkState.Connected)
            this.Stop();


    }

    protected override void Finish(bool success)
    {
        connection.Stop();
        connection_isFullyConnected = false;
    }

    private bool ConnectToServer()
    {
        while ( connection_connectAttempts < CONNECTATTEMPTS_MAX )
        {
            connection_connectAttempts += 1;

            //Start new connection
            if (connection != null)
                connection.Stop();
            connection = new NetConnection(ClientToMainPackets.ReadBuffer, connection_endPoint, TIMEOUT_CONNECT, TIMEOUT_RECEIVE);
            connection.Start();

            //Wait to connect or fail
            while (connection.State == NetConnection.NetworkState.Waiting ||
                   connection.State == NetConnection.NetworkState.Connecting)
            { }

            //Could not connect, try again
            if (connection.State != NetConnection.NetworkState.Connected)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Connection timed out...trying again.");
                continue;
            }

            //Send authentication
            connection.SendPacket(new ClientToMainPackets.LoginAttempt_m(GameVersion.Build, username, password));

            //Wait for reply
            Packet newP = null;
            while ( (newP = connection.GetPacket()) == null )
            {
                if (connection.State != NetConnection.NetworkState.Connected)
                    return false;
            }

            //Read reply from server
            if ((ClientToMainPackets.PacketType)newP.Type == ClientToMainPackets.PacketType.LoginResult_c)
            {
                ClientToMainPackets.LoginResult_c p = (ClientToMainPackets.LoginResult_c)newP;

                if (p.result == ClientToMainPackets.LoginResult_c.LoginResult.Success)
                {
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verified successfully.");
                    connection_isFullyConnected = true;
                    return true;
                }
                else
                {
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verification failed! -> " + p.result.ToString());
                    return false;
                }
            }
            else
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "Received wrong packet while verifying!");
                return false;
            }
        }
        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "Max authication attempts reached.");
        return false;
    }

    /// <summary>
    /// Returns true if there is an active connection and it is verified by the server.
    /// </summary>
    public bool IsFullyConnected
    {
        get
        {
            return connection_isFullyConnected;
        }
    }
}