using DummyClient;
using Network;
using ServerCore;
using System;
using System.Collections;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private ServerSession _session = new ServerSession();

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

    private void Update()
    {
        var packets = PacketQueue.Instance.PopAll();

        foreach(var packet in packets)
        {
            if (packet != null)
                PacketManager.Instance.HandlePacket(_session, packet);
        }
        
    }

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
