// https://github.com/NumesSanguis/Basic-Unity3D-Python-server/blob/master/BasicUnityPython/Assets/Scripts/TCPTestServer.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 

public class SocketServer : MonoBehaviour
{
    public string host = "127.0.0.1";
    public int port = 13296;

	private TcpListener tcpListener; 
	private Thread tcpListenerThread;  	
	// private TcpClient connectedTcpClient; 	

	int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
    }

    // Update is called once per frame
    void Update()
    {
    }

	void OnDestroy() {
		tcpListenerThread.Abort();
	}

	private void ListenForIncommingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Parse(host), port); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[5*4];  			
			while (true) {
                using (TcpClient connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading 					
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        // Read incomming stream into byte arrary. 						
                        while ((stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new float[bytes.Length / 4];
                            // Array.Copy(bytes, 0, incommingData, 0, length);  							
                            Buffer.BlockCopy(bytes, 0, incommingData, 0, bytes.Length);
                            // Convert byte array to JSON message. 							
                            string msg = "[ ";
                            foreach (var item in incommingData)
                            {
                                msg += item.ToString("0.00") + ", ";
                            }
                            Debug.Log(msg + "]");
                        }
                    }
                } 					
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}
}