using System;
using System.Net;

using Extant;
using Extant.Networking;

using SharedComponents;

public static class WorldServerConnection
{
    private static int TIMEOUT_CONNECT = 5000;
    private static int TIMEOUT_RECEIVE = 180000;

    public static ThreadTask<NetConnection> ConnectAsync(IPEndPoint endPoint, String username, Int32 password)
    {
        ThreadTask<NetConnection> t = new ThreadTask<NetConnection>(Connect,
            new object[]{
                endPoint,
                username,
                password
            });
        t.Start();
        return t;
    }

    private static NetConnection Connect(ThreadTaskParams parameters)
    {
        IPEndPoint endPoint = (IPEndPoint)parameters[0];
        String username = (String)parameters[1];
        Int32 password = (Int32)parameters[2];

        //Start new connection
        NetConnection connection = new NetConnection(ClientToWorldPackets.ReadBuffer, endPoint, TIMEOUT_CONNECT, TIMEOUT_RECEIVE);
        connection.Start();

        //Wait to connect or fail
        while (connection.State == NetConnection.NetworkState.Waiting ||
                connection.State == NetConnection.NetworkState.Connecting)
        { }

        //Could not connect, try again
        if (connection.State != NetConnection.NetworkState.Connected)
        {
            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Connection attempt timed out.");
            return null;
        }

        //Send authentication
        connection.SendPacket(new ClientToWorldPackets.Verify_Details_g(GameVersion.Build, username, password));

        //Wait for reply
        Packet newP = null;
        while ((newP = connection.GetPacket()) == null)
        {
            if (connection.State != NetConnection.NetworkState.Connected)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Connection disconnected waiting for reply from server.");
                return null;
            }
        }

        //Read reply from server
        if (newP.Type == (int)ClientToWorldPackets.PacketType.Verify_Result_c)
        {
            ClientToWorldPackets.Verify_Result_c p = (ClientToWorldPackets.Verify_Result_c)newP;

            if (p.returnCode == ClientToWorldPackets.Verify_Result_c.VerifyReturnCode.Success)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verified successfully!");
                return connection;
            }
            else
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Verification failed! -> " + p.returnCode.ToString());
                return null;
            }
        }
        else
        {
            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Fatal, "Received wrong packet while verifying.");
            return null;
        }
    }
}
