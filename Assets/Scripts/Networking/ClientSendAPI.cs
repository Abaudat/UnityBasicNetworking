using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSendAPI : MonoBehaviour {

    public ClientNetworkingBase client;

    public void SendDebugMessage(string s)
    {
        byte[] data = MessageBuilder.BuildDebugMessage(s);
        client.Send(data);
    }
}
