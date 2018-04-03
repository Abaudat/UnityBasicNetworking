using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerMessageParser : MonoBehaviour {

    public void ParseMessage(byte[] message, int connectionId)
    {
        byte[] size = new byte[sizeof(int)];
        Buffer.BlockCopy(message, 0, size, 0, sizeof(int));
        byte[] data = new byte[BitConverter.ToInt32(size, 0)];
        Buffer.BlockCopy(message, sizeof(int) + 1, data, 0, data.Length);
        switch (message[sizeof(int)])
        {
            case (byte)NetworkingFlags.Flags.DEBUG_MESSAGE:
                OnDebugMessage(data, connectionId);
                break;
            default:
                Debug.Log("WARNING: Unknown byte flag " + message[sizeof(int)] + " .");
                break;
        }
    }

    void OnDebugMessage(byte[] data, int connectionId)
    {
        string message = MessageDecoder.DecodeDebugMessage(data);
        Debug.Log("Recieved debug message from client number " + connectionId + " : " + message);
    }
}
