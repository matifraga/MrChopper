using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

public class Channel {

    Thread receiveThread;
    UdpClient client;
    IPEndPoint player;

    int port;
    byte[] receivedData;

    public void ReceiveData() {
        try {
            client = new UdpClient(port);
            while (Thread.CurrentThread.IsAlive) {
                receivedData = client.Receive(ref player);
            }
        } catch(Exception e) {
            Debug.Log("Error receiving data");
        }
    }

    public void SendData(byte[] data) {
        client.Send(data, data.Length, player);

        Debug.Log("Sending data... " + data);
    }

    public void Open(int port, String ip) {
        this.port = port;
        player = new IPEndPoint(IPAddress.Parse(ip), port);
        client.Connect(player);

        receiveThread = new Thread(new ThreadStart(ReceiveData)) {
            IsBackground = true
        };
        receiveThread.Start();

        Debug.Log("Open thread in port " + port + " and ip " + ip);
    }

    public void Close() {
        if (receiveThread != null) {
            receiveThread.Abort();
            receiveThread = null;
            Debug.Log("Close thread");
        }
        if (client != null) {
            client.Close();
            client = null;
            Debug.Log("Close client");
        }

    }

    public bool IsOpen() {
        return client != null;
    }
}
