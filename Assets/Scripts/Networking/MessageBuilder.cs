using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to build byte arrays to send through the networking methods from data.
/// </summary>
public static class MessageBuilder {

    /// <summary>
    /// Build a debug message.
    /// </summary>
    /// <param name="s">String to send</param>
    /// <returns>Byte array ready to send</returns>
    public static byte[] BuildDebugMessage(string s)
    {
        byte[] data = StringToByteArray(s);
        return FlagMessage(data, NetworkingFlags.Flags.DEBUG_MESSAGE);
    }

    /// <summary>
    /// Flag a byte array with a flag from the enum NetworkingFlags.Flags.
    /// </summary>
    /// <param name="data">Message to flag</param>
    /// <param name="flag">Flag to use</param>
    /// <returns>Flagged message</returns>
    static byte[] FlagMessage(byte[] data, NetworkingFlags.Flags flag)
    {
        byte[] flaggedData = new byte[data.Length + sizeof(int) + 1];

        Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, flaggedData, 0, sizeof(int));
        flaggedData[sizeof(int)] = (byte)flag;
        Buffer.BlockCopy(data, 0, flaggedData, sizeof(int) + 1, data.Length);

        return flaggedData;
    }

    /// <summary>
    /// Helper method to convert a string to a byte array.
    /// </summary>
    /// <param name="s">String to convert</param>
    /// <returns>Converted byte array</returns>
    static byte[] StringToByteArray(string s)
    {
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// Helper method to convert a Vector3 to a byte array.
    /// </summary>
    /// <param name="vector">Vector3 to convert</param>
    /// <returns>Converted byte array</returns>
    static byte[] Vector3ToByteArray(Vector3 vector)
    {
        byte[] bytes = new byte[sizeof(float) * 3];

        Buffer.BlockCopy(BitConverter.GetBytes(vector.x), 0, bytes, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(vector.y), 0, bytes, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(vector.z), 0, bytes, 2 * sizeof(float), sizeof(float));

        return bytes;
    }
}
