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
        worker = new ThreadWorker(GetPoseTask());
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

    IEnumerator GetTimeFromPythonTask(string ip = "localhost", int port = 12345)
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
    IEnumerator GetPoseTask(string ip = "localhost", int port = 12345)
    {
        client = new SocketClient(ip, port);
        if (client.ConnectToTcpServer()) {
            while (client.running && client.Available()) {
                int length = new int();
                bool received = client.ListenForLength(ref length);

                if (received) {
                    client.SendMessage(client.stream, "true");
                    int cnt = (int) length/(3*4);

                    Byte[] data = new Byte[length];
                    received = client.ListenForData(ref data);
                
                    if (received)
                    {
                        List<Vector3> coords = new List<Vector3>();
                        for (int i = 0; i < cnt; i++)
                        {
                            Vector3 vec = new Vector3();
                            vec[0] = BitConverter.ToSingle(data, i*3*4);
                            vec[1] = BitConverter.ToSingle(data, i*3*4+4);
                            vec[2] = BitConverter.ToSingle(data, i*3*4+2*4);
                            coords.Add(vec);
                        }
                        client.SendMessage(client.stream, "true");
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