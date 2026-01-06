using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    private NetworkManager _network = null;
    // Start is called before the first frame update
    void Start()
    {
        _network = GameObject.Find("Network").GetComponent<NetworkManager>();

        StartCoroutine("CoSendPacket");
    }

    private IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range(-10, 10);
            movePacket.posY = 0;
            movePacket.posZ = UnityEngine.Random.Range(-10, 10);

            _network.Send(movePacket.Write());
        }
    }
}
