using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour
{
    private TcpClient client; 
    private Thread clientReceiveThread; 
    private volatile bool running = false;

    public String ip = "localhost";
    public int port = 13296;

    // Use this for initialization     
    void Start () {
        ConnectToTcpServer();     
    }
    // Update is called once per frame
    void Update () {         
    }      

    void SendMessage(NetworkStream stream, String msg) {
        Byte[] sendBytes = Encoding.UTF8.GetBytes (msg);
        if (stream.CanWrite) {
            stream.Write(sendBytes, 0, sendBytes.Length);
        }
    }

    void OnDestroy() {
        running = false;
    }
    /// <summary>
    /// Setup socket connection.     
    /// </summary>
    private void ConnectToTcpServer () {         
        try {
            clientReceiveThread = new Thread (new ThreadStart(ListenForData));             
            clientReceiveThread.IsBackground = true;             
            clientReceiveThread.Start();
        }         
        catch (Exception e) {             
            Debug.Log("On client connect exception " + e);         
        }     
    }      
    /// <summary>     
    /// Runs in background clientReceiveThread; Listens for incomming data.     
    /// </summary>     
    private void ListenForData() {         
        running = true;
        try {             
            client = new TcpClient(ip, port);              
            Byte[] bytes = new Byte[25*4];
            while (running) {
                // Get a stream object for reading                 
                using (NetworkStream stream = client.GetStream()) {                     
                    int length;
                    // Read incomming stream into byte arrary.                     
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0 && running) {
                        var incommingData = new float[bytes.Length/4]; 
                        // Array.Copy(bytes, 0, incommingData, 0, length);                         
                        Buffer.BlockCopy(bytes, 0, incommingData, 0, bytes.Length);
                        // Convert byte array to string message.                         
                        // Debug.Log("[ " + item.ToString("00:00:00") + "]");
                        String msg = System.Text.Encoding.UTF8.GetString(bytes, 0, length);
                        // GameObject.Find("Timer").GetComponent<Text>().text = msg;
                        Debug.Log("Current Time [" + msg + "]");

                        msg = "test";
                        SendMessage(stream, msg);
                    }                 
                }             
            }         
            client.Close();
        }         
        catch (SocketException socketException) {             
            Debug.Log("Socket exception: " + socketException);         
        }     
    }      
}