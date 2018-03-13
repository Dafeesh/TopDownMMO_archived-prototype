using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extant.Networking
{
    public delegate void Delegate_PacketDistribute<T>(T packet);

    public interface IPacketDistributor : IDisposable
    {
        bool DistributePacket(ref List<byte> buffer);
    }
}
