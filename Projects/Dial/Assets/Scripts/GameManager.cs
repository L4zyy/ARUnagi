using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;

public class GameManager : MonoBehaviour
{
    private ThreadWorker worker = null;

    private SocketClient client = null;

    void Start() {
        worker = new ThreadWorker(GetStringFromPythonTask());
        worker.Start();
        worker.Resume();
    }

    // Update is called once per frame
    void Update() {
    }

    void OnDestroy() {
        if(worker != null)
            worker.Abort();
        if (client != null) {
            if (client.stream != null)
                client.stream.Close();
            client.Close();
        }
    }

    IEnumerator TestTask() {
        while (true) {
            Debug.Log("Test...");
            Debug.Log(Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1000);
            yield return null;
        }
    }

    IEnumerator GetStringFromPythonTask(string ip = "localhost", int port = 12345)
    {
        client = new SocketClient(ip, port);
        if (client.ConnectToTcpServer()) {
            while (client.running && client.Available()) {
                string msg = "";

                int length = new int();
                bool received = client.ListenForLength(ref length);
                if (received) {
                    client.SendMessage(client.stream, "true");

                    Byte[] data = new Byte[length*4];
                    received = client.ListenForData(ref data);
                
                    if (received)
                    {
                        msg = System.Text.Encoding.UTF8.GetString(data, 0, length);

                        client.SendMessage(client.stream, "true");

                        Debug.Log("Current Time [" + msg + "]");
                    } else {
                        client.SendMessage(client.stream, "false");
                    }

                } else {
                    client.SendMessage(client.stream, "false");
                }

                yield return null;
            }
            client.Close();
            Debug.Log("Client Closed...");
        }      
    }
}

