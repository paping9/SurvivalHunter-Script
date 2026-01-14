using System.Collections.Generic;

namespace Network
{
    public interface IPacketQueue
    {
        /// <summary>
/// Enqueues the given packet for later retrieval from the queue.
/// </summary>
/// <param name="packet">The packet to add to the queue.</param>
void Push(IPacket packet);
        /// <summary>
/// Removes and returns a single packet from the queue.
/// </summary>
/// <returns>The packet removed from the queue.</returns>
IPacket Pop();
        /// <summary>
/// Removes and returns all packets currently queued.
/// </summary>
/// <returns>A list containing all dequeued <see cref="IPacket"/> instances in queue order; an empty list if the queue had no packets.</returns>
List<IPacket> PopAll();
    }
}