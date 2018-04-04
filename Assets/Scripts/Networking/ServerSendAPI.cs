using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used by the actual game server to send messages to clients.
/// </summary>
public class ServerSendAPI : MonoBehaviour {

    public ServerNetworkingBase server;

    /// <summary>
    /// Send a debug message that will show up in Debug.Log to a client.
    /// </summary>
    /// <param name="s">Message to send</param>
    /// <param name="connectionId">Connection id of the client</param>
	public void SendDebugMessage(string s, int connectionId)
    {
        byte[] data = MessageBuilder.BuildDebugMessage(s);
        server.SendToClient(connectionId, data);
    }
}
