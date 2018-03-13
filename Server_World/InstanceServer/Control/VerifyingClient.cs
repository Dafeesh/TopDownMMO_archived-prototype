using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

using Extant;
using Extant.Networking;

using SharedComponents.Global;

namespace InstanceServer.Control
{
    public class VerifyingClient : ILogging , IDisposable
    {
        private const Int32 RECEIVE_TIMEOUT = 3000;

        private NetConnection connection;
        private IPacketDistributor connection_distributor;

        private Stopwatch receiveTimer = new Stopwatch();

        private Int32? verifyBuild = null;
        private String verifyUsername = null;
        private Int32? verifyPasswordToken = null;

        private bool _isVerified = false;
        private bool _isDisposed = false;
        private DebugLogger _log = new DebugLogger("AuthClient");

        public VerifyingClient(TcpClient tcpClient)
        {
            this.connection_distributor = new ClientToInstancePackets.Distribution()
            {
                out_Verify_Details_i = OnReceive_Verify_Details_i
            };

            this.connection = new NetConnection(tcpClient);
            this.connection.Start();

            this.receiveTimer.Start();

            this.Log.MessageLogged += Console.WriteLine;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (connection != null)
                    connection.Dispose();
            }
        }

        public void HandleVerification()
        {
            if (receiveTimer.ElapsedMilliseconds > RECEIVE_TIMEOUT)
            {
                Log.Log("Timed out while varifying.");
                connection.Dispose();
            }
            else if (IsConnected && !IsVerified)
            {
                bool gotPacket = connection.DistributePacket(connection_distributor);

                if (gotPacket && !IsVerified)
                {
                    Log.Log("Received wrong packet while authenticating.");
                    connection.Dispose();
                }
            }
        }

        private void OnReceive_Verify_Details_i(ClientToInstancePackets.Verify_Details_i p)
        {
            Log.Log("Got authentication.");

            verifyBuild = p.build;
            verifyUsername = p.username;
            verifyPasswordToken = p.password;

            IsVerified = true;
        }

        public bool IsConnected
        {
            get
            {
                return (connection != null) && (connection.State == NetConnection.NetworkState.Active);
            }
        }

        public Boolean IsVerified
        {
            get
            {
                return _isVerified;
            }

            private set
            {
                _isVerified = value;
            }
        }

        public Int32 VerifyBuild
        {
            get
            {
                return verifyBuild.Value;
            }
        }

        public String VerifyUsername
        {
            get
            {
                return verifyUsername;
            }
        }

        public Int32 VerifyPasswordToken
        {
            get
            {
                return verifyPasswordToken.Value;
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }
        }
    }
}
