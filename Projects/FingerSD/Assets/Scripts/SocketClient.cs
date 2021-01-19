using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour
{
	private TcpClient socketConnection; 
	private Thread clientReceiveThread; 

	// Use this for initialization 	
	void Start () {
		ConnectToTcpServer();     
	}  	
	// Update is called once per frame
	void Update () {         
		// if (Input.GetKeyDown(KeyCode.Space)) {             
		// 	SendMessage();         
		// }     
	}  	

	void SendMessage() {
		using (NetworkStream stream = socketConnection.GetStream()) { 					
			Byte[] sendBytes = Encoding.UTF8.GetBytes ("test");
            stream.Write (sendBytes, 0, sendBytes.Length);
		} 			
	}

	void OnDestroy() {
		clientReceiveThread.Abort();
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
		try { 			
			socketConnection = new TcpClient("localhost", 13296);  			
			Byte[] bytes = new Byte[5*4];
			while (true) {
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
						var incommingData = new float[bytes.Length/4]; 
						// Array.Copy(bytes, 0, incommingData, 0, length); 						
						Buffer.BlockCopy(bytes, 0, incommingData, 0, bytes.Length);
						// Convert byte array to string message. 						
                        string msg = "[ ";
						foreach (var item in incommingData)
						{
                            msg += item.ToString("0.00") + ", ";
						}
                        Debug.Log(msg + "]");

						Byte[] sendBytes = Encoding.UTF8.GetBytes ("test");
   			         	if (stream.CanWrite) {
								stream.Write(sendBytes, 0, sendBytes.Length);
								Debug.Log("Send back recieved msg.");
							}
					} 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
}