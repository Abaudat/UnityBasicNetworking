using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageDecoder {

    public static string DecodeDebugMessage(byte[] data)
    {
        return ByteArrayToString(data);
    }

    public static int DecodeIdMessage(byte[] data)
    {
        return BitConverter.ToInt32(data, 0);
    }

	static string ByteArrayToString(byte[] data)
    {
        return System.Text.Encoding.UTF8.GetString(data);
    }

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
