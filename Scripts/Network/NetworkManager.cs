using DummyClient;
using Network;
using ServerCore;
using System;
using System.Net;
using UnityEngine;
using VContainer;

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session = new ServerSession();
    private IPacketQueue _packetQueue;

    /// <summary>
    /// Injects and stores the packet queue used to retrieve incoming network packets.
    /// </summary>
    /// <param name="packetQueue">The packet queue implementation to use for processing incoming packets.</param>
    [Inject]
    public void Construct(IPacketQueue packetQueue)
    {
        _packetQueue = packetQueue;
    }

    /// <summary>
    /// Sends the provided byte segment through the active server session.
    /// </summary>
    /// <param name="sendBuff">The segment of bytes to transmit over the network.</param>
    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }
    // Start is called before the first frame update
    void Start()
    {
        //string host = Dns.GetHostName();
        //IPHostEntry ipHost = Dns.GetHostEntry(host);
        //IPAddress ipAddr = ipHost.AddressList[0];
        //IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        //Connector connector = new Connector();
        //connector.Connect(endPoint
        //    , () => { return _session; }
        //    , 1);

        Test();
    }

    private void OnDestroy()
    {
        _session.Disconnect();
    }

    /// <summary>
    /// Called once per frame to process all pending network packets.
    /// </summary>
    /// <remarks>
    /// Retrieves all packets from the injected packet queue and passes each non-null packet to the packet handling system for processing.
    /// </remarks>
    private void Update()
    {
        var packets = _packetQueue.PopAll();

        foreach (var packet in packets)
        {
            if (packet != null)
                PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    /// <summary>
    /// Computes the total number of decimal digits required to write all integers from 1 through 15 and logs the result.
    /// </summary>
    private void Test()
    {
        int n = 15;
        int count = 0;

        for(int i = 1; i <= n; i++)
        {
            int tmp = i;

            while(tmp > 0)
            {
                tmp /= 10;
                count++;
            }
        }

        Debug.Log(count);
    }

}