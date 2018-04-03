using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerNetworkingBase : MonoBehaviour {

    public ServerMessageParser parser;

    public const int MAX_CONNECTIONS = 100, BUFFER_SIZE = 2048, SERVER_PORT = 8642;

    private int reliableChannelId, stateUpdateChannelId, hostId;

    private bool isInit = false;

    private byte error;

    private List<int> connections = new List<int>();

	public void Init() {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelId = config.AddChannel(QosType.Reliable);
        stateUpdateChannelId = config.AddChannel(QosType.StateUpdate);
        HostTopology topology = new HostTopology(config, MAX_CONNECTIONS);
        hostId = NetworkTransport.AddHost(topology, SERVER_PORT);
        isInit = true;
    }

    public void SendToClient(int connectionId, byte[] data)
    {
        if(connections.Contains(connectionId))
        {
            NetworkTransport.Send(hostId, connectionId, reliableChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
        else
        {
            Debug.Log("WARNING: Trying to send to disconnected client " + connectionId);
        }
    }

    public void SendUpdateToClient(int connectionId, byte[] data)
    {
        if (connections.Contains(connectionId))
        {
            NetworkTransport.Send(hostId, connectionId, stateUpdateChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
        else
        {
            Debug.Log("WARNING: Trying to send to disconnected client " + connectionId);
        }
    }

    public void SendToAllClients(byte[] data)
    {
        foreach(int connection in connections)
        {
            NetworkTransport.Send(hostId, connection, reliableChannelId, data, data.Length, out error);
            if ((NetworkError)error != NetworkError.Ok)
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
    }

    public void SendUpdateToAllClient(byte[] data)
    {
        foreach (int connection in connections)
        {
            if (connections.Contains(connection))
            {
                NetworkTransport.Send(hostId, connection, stateUpdateChannelId, data, data.Length, out error);
                if ((NetworkError)error != NetworkError.Ok)
                {
                    Debug.Log("WARNING: " + (NetworkError)error);
                }
            }
            else
            {
                Debug.Log("WARNING: Trying to send to disconnected client " + connection);
            }
        }
    }

    void FixedUpdate () {
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
            if((NetworkError)error == NetworkError.Ok)
            {
                switch (recData)
                {
                    case NetworkEventType.Nothing:         
                        break;
                    case NetworkEventType.ConnectEvent:
                        OnClientConnection(connectionId);
                        break;
                    case NetworkEventType.DataEvent:
                        OnDataReceived(connectionId, recBuffer);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        OnClientDisconnect(connectionId);
                        break;
                }
            }
            else if(((NetworkError)error) == NetworkError.Timeout)
            {
                OnClientDisconnect(connectionId);
                Debug.Log("Timeout from client: " + connectionId);
            }
            else
            {
                Debug.Log("WARNING: " + (NetworkError)error);
            }
        }
    }

    void OnClientConnection(int connectionId)
    {
        if (connections.Contains(connectionId))
        {
            Debug.Log("SERVER: Reconnection from connection number " + connectionId + ".");
        }
        else
        {
            Debug.Log("SERVER: New connection from connection number " + connectionId + ".");
            connections.Add(connectionId);
        }
    }

    void OnDataReceived(int connectionId, byte[] buffer)
    {
        parser.ParseMessage(buffer, connectionId);
    }

    void OnClientDisconnect(int connectionId)
    {
        Debug.Log("SERVER: Connection number" + connectionId + " disconnected.");
        connections.Remove(connectionId);
    }
}
