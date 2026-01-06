using System;
using System.Collections.Generic;
using Utils;

namespace Network
{
    public class PacketQueue : Singleton<PacketQueue>
    {
        private Queue<IPacket> _packetQueue = new Queue<IPacket>();
        private object _lock = new object();

        public void Push(IPacket packet)
        {
            lock (_lock)
            {
                _packetQueue.Enqueue(packet);
            }
        }

        public IPacket Pop()
        {
            lock (_lock)
            {
                if (_packetQueue.Count == 0) return null;

                return _packetQueue.Dequeue();
            }
        }

        public List<IPacket> PopAll()
        {
            List<IPacket> list = new List<IPacket>();

            lock (_lock)
            {
                while(_packetQueue.Count > 0)
                {
                    list.Add(_packetQueue.Dequeue());
                }
            }

            return list;
        }
    }
}
