using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to decode message from byte array to usable data.
/// </summary>
public static class MessageDecoder {

    /// <summary>
    /// Decode a debug message.
    /// </summary>
    /// <param name="data">Data to decode</param>
    /// <returns>The message that was sent by the client</returns>
    public static string DecodeDebugMessage(byte[] data)
    {
        return ByteArrayToString(data);
    }

    /// <summary>
    /// Helper methode to convert a byte array to a string.
    /// </summary>
    /// <param name="data">Byte array to convert</param>
    /// <returns>Converted string</returns>
	static string ByteArrayToString(byte[] data)
    {
        return System.Text.Encoding.UTF8.GetString(data);
    }

    /// <summary>
    /// Helper method to convert a byte array to a Vector3.
    /// </summary>
    /// <param name="data">Byte array to convert</param>
    /// <returns>Converted Vector3</returns>
    static Vector3 ByteArrayToVector3(byte[] data)
    {
        byte[] buff = data;
        Vector3 vect = Vector3.zero;
        vect.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        vect.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
        vect.z = BitConverter.ToSingle(buff, 2 * sizeof(float));
        return vect;
    }
}
