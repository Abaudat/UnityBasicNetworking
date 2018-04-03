using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSendAPI : MonoBehaviour {

    public ServerNetworkingBase server;

	public void SendDebugMessage(string s, int connectionId)
    {
        byte[] data = MessageBuilder.BuildDebugMessage(s);
        server.SendToClient(connectionId, data);
    }

    public void SendId(int id, int connectionId)
    {
        byte[] data = MessageBuilder.BuildIdMessage(id);
        server.SendToClient(connectionId, data);
    }
}
