using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that parses all networking messages received by the server. Add cases in ParseMessage for all new Flags in NetworkingFlags.
/// </summary>
public class ServerMessageParser : MonoBehaviour {

    /// <summary>
    /// Parse a message.
    /// </summary>
    /// <param name="message">Data received</param>
    /// <param name="connectionId">Client's id</param>
    public void ParseMessage(byte[] message, int connectionId)
    {
        byte[] size = new byte[sizeof(int)];
        Buffer.BlockCopy(message, 0, size, 0, sizeof(int));
        byte[] data = new byte[BitConverter.ToInt32(size, 0)];
        Buffer.BlockCopy(message, sizeof(int) + 1, data, 0, data.Length);
        switch (message[sizeof(int)]) // Add cases to this switch for all different types of message the server can receive.
        {
            case (byte)NetworkingFlags.Flags.DEBUG_MESSAGE:
                OnDebugMessage(data, connectionId);
                break;
            default:
                Debug.LogWarning("WARNING: Unknown byte flag " + message[sizeof(int)] + " .");
                break;
        }
    }

    /// <summary>
    /// Called when the server receives a debug message.
    /// </summary>
    /// <param name="data">Data received</param>
    /// <param name="connectionId">Client's id</param>
    void OnDebugMessage(byte[] data, int connectionId)
    {
        string message = MessageDecoder.DecodeDebugMessage(data);
        Debug.Log("Recieved debug message from client number " + connectionId + " : " + message);
    }
}
