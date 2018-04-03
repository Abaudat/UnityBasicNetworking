using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetworkingBase : MonoBehaviour {

    public ClientMessageParser parser;

    public const string SERVER_ADDRESS = "127.0.0.1";
    public const int SERVER_PORT = 8642, BUFFER_SIZE = 2048;

    private int reliableChannelId, stateUpdateChannelId, hostId, connectionId;
    private byte error;

    private bool isInit = false, isConnected = false;

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
            Debug.Log("WARNING: " + (NetworkError)error);
        }
        else
        {
            isInit = true;
        }
    }

    public void Send(byte[] data)
    {
        if (isConnected)
        {
            NetworkTransport.Send(hostId, connectionId, reliableChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
    }

    public void SendUpdate(byte[] data)
    {
        if (isConnected)
        {
            NetworkTransport.Send(hostId, connectionId, stateUpdateChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
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
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
    }

    void OnConnection()
    {
        Debug.Log("CLIENT: Connected to server.");
        isConnected = true;
    }

    void OnDataReceived(byte[] buffer)
    {
        parser.ParseMessage(buffer, 0);
    }

    void OnConnectRequestFailed()
    {
        Debug.Log("CLIENT: Couldn't connect to server.");
    }
}
