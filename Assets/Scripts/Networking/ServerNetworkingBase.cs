using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class that implements the LLAPI methods to send and receive for the server.
/// </summary>
public class ServerNetworkingBase : MonoBehaviour {

    public ServerMessageParser parser;

    public const int MAX_CONNECTIONS = 100, // Maximum number of clients that can connect at the same time.
        BUFFER_SIZE = 2048, // Size of the receive buffer.
        SERVER_PORT = 8642; // Port the socket will use.

    private int reliableChannelId, // Use this channel for reliable messages.
        stateUpdateChannelId,  // Use this channel for state update messages (position, rotation, ...)
        hostId;

    private bool isInit = false;

    private byte error;

    private List<int> connections = new List<int>();

    /// <summary>
    /// Initialize the server. It cannot be used before this is called.
    /// </summary>
	public void Init() {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelId = config.AddChannel(QosType.Reliable);
        stateUpdateChannelId = config.AddChannel(QosType.StateUpdate);
        HostTopology topology = new HostTopology(config, MAX_CONNECTIONS);
        hostId = NetworkTransport.AddHost(topology, SERVER_PORT);
        isInit = true;
    }

    /// <summary>
    /// Send data to a specific client on the reliable channel.
    /// </summary>
    /// <param name="connectionId">Client's connection id</param>
    /// <param name="data">Data to send</param>
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

    /// <summary>
    /// Send data to a specific client on the state update channel.
    /// </summary>
    /// <param name="connectionId">Client's connection id</param>
    /// <param name="data">Data to send</param>
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

    /// <summary>
    /// Send data to all clients on the reliable channel.
    /// </summary>
    /// <param name="data">Data to send</param>
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

    /// <summary>
    /// Send data to all clients on the state update channel.
    /// </summary>
    /// <param name="data">Data to send</param>
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

    /// <summary>
    /// Called on client connection/reconnection.
    /// </summary>
    /// <param name="connectionId">Client's connection id</param>
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

    /// <summary>
    /// Called when data is received.
    /// </summary>
    /// <param name="connectionId">Client's connection id</param>
    /// <param name="buffer">Buffer containing received data</param>
    void OnDataReceived(int connectionId, byte[] buffer)
    {
        parser.ParseMessage(buffer, connectionId);
    }

    /// <summary>
    /// Called when client specifically asks to disconnect (does not always happen!)
    /// </summary>
    /// <param name="connectionId">Client's connection id</param>
    void OnClientDisconnect(int connectionId)
    {
        Debug.Log("SERVER: Connection number" + connectionId + " disconnected.");
        connections.Remove(connectionId);
    }
}
