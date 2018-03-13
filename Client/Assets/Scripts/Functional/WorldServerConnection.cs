using System;
using System.Net;

using Extant;
using Extant.Networking;

using SharedComponents;

public class WorldServerConnection : ThreadRun
{
    private static int TIMEOUT_CONNECT = 5000;
    private static int TIMEOUT_RECEIVE = 10000;
    private static int CONNECTATTEMPTS_MAX = 3;

    private String username;
    private Int32 password;

    private NetConnection connection;
    private IPEndPoint connection_endPoint;
    private Int32 connection_connectAttempts = 0;
    private bool connection_fullyConnected = false;

    public WorldServerConnection(IPEndPoint endPoint, string username, int password)
        : base("WorldServerConnection")
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
        connection_fullyConnected = false;
    }

    private bool ConnectToServer()
    {
        while ( connection_connectAttempts < CONNECTATTEMPTS_MAX )
        {
            connection_connectAttempts += 1;

            //Start new connection
            if (connection != null)
                connection.Stop();
            connection = new NetConnection(ClientToWorldPackets.ReadBuffer, connection_endPoint, TIMEOUT_CONNECT, TIMEOUT_RECEIVE);
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
            connection.SendPacket(new ClientToWorldPackets.Verify_Details_g(GameVersion.Build, username, password));

            //Wait for reply
            Packet newP = null;
            while ((newP = connection.GetPacket()) == null)
            {
                if (connection.State != NetConnection.NetworkState.Connected)
                    return false;
            }

            //Read reply from server
            if (newP.Type == (int)ClientToWorldPackets.PacketType.Verify_Result_c)
            {
                ClientToWorldPackets.Verify_Result_c p = (ClientToWorldPackets.Verify_Result_c)newP;

                if (p.returnCode == ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.Success)
                {
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verified successfully.");
                    connection_fullyConnected = true;
                    return true;
                }
                else
                {
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verification failed! -> " + p.returnCode.ToString());
                    return false;
                }
            }
            else
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Fatal, "Received wrong packet while verifying!");
                return false;
            }
        }
        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "Max authication attempts reached.");
        return false;
    }

    /// <summary>
    /// Returns true if there is an active connection and is verified by the server.
    /// </summary>
    public bool IsFullyConnected
    {
        get
        {
            return connection_fullyConnected;
        }
    }
}
