using System;

using Extant.Networking;

namespace SharedComponents.Server
{
    public class ClientIDPacketPair
    {
        public UInt32 ClientID;
        public Packet Packet;

        public ClientIDPacketPair(UInt32 clientID, Packet packet)
        {
            this.ClientID = clientID;
            this.Packet = packet;
        }
    }
}
