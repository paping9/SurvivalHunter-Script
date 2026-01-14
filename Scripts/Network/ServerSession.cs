using Network;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{


    public class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected {endPoint}");

        }

        /// <summary>
        /// Handles incoming packet data received from the network.
        /// </summary>
        /// <param name="buffer">A segment of bytes containing the received packet data.</param>
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            //PacketManager.Instance.OnRecvPacket(this, buffer, (session, packet) => PacketQueue.Instance.Push(packet));
        }
        /// <summary>
        /// Called after data has been sent to the remote endpoint to notify how many bytes were transmitted.
        /// </summary>
        /// <param name="numOfBytes">The number of bytes successfully sent in the most recent send operation.</param>
        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes! {numOfBytes}");
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected {endPoint}");
        }

    }
}