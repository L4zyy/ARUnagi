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
                string msg = client.ListenForData();

                Debug.Log("Current Time [" + msg + "]");

                msg = "test";
                client.SendMessage(client.stream, msg);
                yield return null;
            }
            client.Close();
        }      
    }
}

