using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageBuilder {

    public static byte[] BuildDebugMessage(string s)
    {
        byte[] data = StringToByteArray(s);
        return FlagMessage(data, NetworkingFlags.Flags.DEBUG_MESSAGE);
    }

    public static byte[] BuildIdMessage(int id)
    {
        byte[] data = BitConverter.GetBytes(id);
        return FlagMessage(data, NetworkingFlags.Flags.GAME_ID);
    }

    static byte[] FlagMessage(byte[] data, NetworkingFlags.Flags flag)
    {
        byte[] flaggedData = new byte[data.Length + sizeof(int) + 1];

        Buffer.BlockCopy(BitConverter.GetBytes(data.Length), 0, flaggedData, 0, sizeof(int));
        flaggedData[sizeof(int)] = (byte)flag;
        Buffer.BlockCopy(data, 0, flaggedData, sizeof(int) + 1, data.Length);

        return flaggedData;
    }

    static byte[] StringToByteArray(string s)
    {
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    static byte[] Vector3ToByteArray(Vector3 vector)
    {
        byte[] bytes = new byte[sizeof(float) * 3];

        Buffer.BlockCopy(BitConverter.GetBytes(vector.x), 0, bytes, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(vector.y), 0, bytes, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(vector.z), 0, bytes, 2 * sizeof(float), sizeof(float));

        return bytes;
    }
}
