using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used by the actual game client to send messages to the server.
/// </summary>
public class ClientSendAPI : MonoBehaviour {

    public ClientNetworkingBase client;

    /// <summary>
    /// Send a debug message that will show up in Debug.Log to the server.
    /// </summary>
    /// <param name="s">Message to send</param>
    public void SendDebugMessage(string s)
    {
        byte[] data = MessageBuilder.BuildDebugMessage(s);
        client.Send(data);
    }
}
