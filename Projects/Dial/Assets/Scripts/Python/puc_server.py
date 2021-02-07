# python unity communication

import socket 
import time

HOST = 'localhost' 
PORT = 13296 
HEADER = 64
FORMAT = 'utf-8'
DISCONNECT_MSG = '!DISCONNECT'

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((HOST,PORT)) 

import numpy as np
data = np.arange(5, dtype=np.float32)
print(data)

s.listen()

while True:
    print("Waiting connection...")
    conn, addr = s.accept() 
    print("Client connected.")

    while True: 
        conn.send(data.tobytes()) 
        msg = conn.recv(HEADER).decode(FORMAT)
        if not msg:
            break
        msg_length = str(msg)
        print(f"[{addr}] {msg}")
        time.sleep(2)
    print("Client disconnected.")