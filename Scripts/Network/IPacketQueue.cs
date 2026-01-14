using System.Collections.Generic;

namespace Network
{
    public interface IPacketQueue
    {
        void Push(IPacket packet);
        IPacket Pop();
        List<IPacket> PopAll();
    }
}
