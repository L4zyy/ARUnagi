# python unity communication

import socket 
import time
import datetime

HOST = 'localhost' 
PORT = 13296 
HEADER = 64
FORMAT = 'utf-8'
DISCONNECT_MSG = '!DISCONNECT'

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.bind((HOST,PORT)) 

import numpy as np

s.listen()

while True:
    print("Waiting connection...")
    conn, addr = s.accept() 
    print("Client connected.")

    while True: 
        t = datetime.datetime.now().strftime("%X")
        print(t)
        conn.send(t.encode(FORMAT)) 
        msg = conn.recv(HEADER).decode(FORMAT)
        if not msg:
            break
        print(f"[{addr}] {msg}")
        time.sleep(2)
    print("Client disconnected.")