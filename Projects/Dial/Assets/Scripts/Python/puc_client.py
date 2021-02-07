# python unity communication
# https://www.youtube.com/watch?v=3QiPPX-KeSc

import socket 

HOST = 'localhost' 
PORT = 13296 
HEADER = 64
FORMAT = 'utf-8'
DISCONNECT_MSG = '!DISCONNECT'

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
s.connect((HOST,PORT)) 

import numpy as np
data = np.arange(5, dtype=np.float32)
print(data)

s.send(data.tobytes())