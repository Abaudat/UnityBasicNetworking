using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class that implements the LLAPI methods to send and receive for the client.
/// </summary>
public class ClientNetworkingBase : MonoBehaviour {

    public ClientMessageParser parser;

    public const string SERVER_ADDRESS = "127.0.0.1";
    public const int SERVER_PORT = 8642, 
        BUFFER_SIZE = 2048; // Size of the receive buffer.

    private int reliableChannelId, // Use this channel for reliable messages.
        stateUpdateChannelId, // Use this channel for state update messages (position, rotation, ...)
        hostId, connectionId;
    private byte error;

    private bool isInit = false, isConnected = false;

    /// <summary>
    /// Initialize the client. It cannot be used before this is called.
    /// </summary>
    public void Init()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelId = config.AddChannel(QosType.Reliable);
        stateUpdateChannelId = config.AddChannel(QosType.StateUpdate);
        HostTopology topology = new HostTopology(config, 10);
        hostId = NetworkTransport.AddHost(topology);
        connectionId = NetworkTransport.Connect(hostId, SERVER_ADDRESS, SERVER_PORT, 0, out error);
        if((NetworkError)error != NetworkError.Ok)
        {
            Debug.LogWarning("WARNING: " + (NetworkError)error);
        }
        else
        {
            isInit = true;
        }
    }

    /// <summary>
    /// Send a message to the server on the reliable channel.
    /// </summary>
    /// <param name="data">Message to send</param>
    public void Send(byte[] data)
    {
        if (isConnected)
        {
            NetworkTransport.Send(hostId, connectionId, reliableChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogWarning("WARNING: " + (NetworkError)error);
            }
        }
        else
        {
            Debug.Log("Cannot send to the server until we connect.");
        }
    }

    /// <summary>
    /// Send a message to the server on the state update channel.
    /// </summary>
    /// <param name="data">Message to send</param>
    public void SendUpdate(byte[] data)
    {
        if (isConnected)
        {
            NetworkTransport.Send(hostId, connectionId, stateUpdateChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.LogWarning("WARNING: " + (NetworkError)error);
            }
        }
        else
        {
            Debug.Log("Cannot send to the server until we connect.");
        }
    }

    void FixedUpdate()
    {
        if (isInit)
        {
            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[BUFFER_SIZE];
            int bufferSize = BUFFER_SIZE;
            int dataSize;
            byte error;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
            if ((NetworkError)error == NetworkError.Ok)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.ConnectEvent:
                        OnConnection();
                        break;
                    case NetworkEventType.DataEvent:
                        OnDataReceived(recBuffer);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        OnConnectRequestFailed();
                        break;
                }
            }
            else
            {
                Debug.LogWarning("WARNING: " + (NetworkError)error);
            }
        }
    }

    /// <summary>
    /// Called upon connection to the server.
    /// </summary>
    void OnConnection()
    {
        Debug.Log("CLIENT: Connected to server.");
        isConnected = true;
    }

    /// <summary>
    /// Called when data is received.
    /// </summary>
    /// <param name="buffer">Received data</param>
    void OnDataReceived(byte[] buffer)
    {
        parser.ParseMessage(buffer);
    }

    /// <summary>
    /// Called upon failure to connect to the server.
    /// </summary>
    void OnConnectRequestFailed()
    {
        Debug.Log("CLIENT: Couldn't connect to server.");
    }
}
