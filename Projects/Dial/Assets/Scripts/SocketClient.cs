using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient
{
    private TcpClient client; 
    private Thread clientReceiveThread; 

    public volatile bool running = false;
    public NetworkStream stream = null;

    public string ip;
    public int port;

    public SocketClient(string _ip = "localhost", int _port = 12345) {
        ip = _ip;
        port = _port;
    }

    public bool Available() {
        try
        {
            return !(client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0);
        }
        catch (SocketException) { return false; }
    }

    public bool ConnectToTcpServer () {         
        try
        {
            Debug.LogFormat("Trying to connect to: {0}:{1}", ip, port);
            client = new TcpClient(ip, port);              
            stream = client.GetStream();
            running = true;
            return true;
        }
        catch (SocketException socketException) {             
            Debug.Log("Socket exception: " + socketException);         
            return false;
        }     
    }      

    public bool ListenForLength(ref int length) {
        int len;
        Byte[] data = new Byte[4];
        if ((len = stream.Read(data, 0, data.Length)) != 0) {
            length = BitConverter.ToInt32(data, 0);
            return true;
        }

        return false;
    }      

    public bool ListenForData(ref Byte[] data) {
        int length;
        if ((length = stream.Read(data, 0, data.Length)) != 0 && running) {
            return true;
        }

        return false;
    }      

    public void SendMessage(NetworkStream stream, string msg) {
        Byte[] sendBytes = Encoding.UTF8.GetBytes (msg);
        if (stream.CanWrite) {
            stream.Write(sendBytes, 0, sendBytes.Length);
        }
    }

    public void Close() {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }

    void OnDestroy() {
        running = false;
    }

}