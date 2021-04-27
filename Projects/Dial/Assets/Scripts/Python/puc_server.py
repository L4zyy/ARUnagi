# python unity communication

import socket 
import time
import datetime
import signal

import sys
sys.path.append('../../../../../')
import aru

HOST = 'localhost' 
PORT = 12345 
HEADER = 64
FORMAT = 'utf-8'
DISCONNECT_MSG = '!DISCONNECT'

def sendTime(conn):
    tm = datetime.datetime.now().strftime("%X")
    print(tm)
    conn.send(tm.encode(FORMAT)) 
    msg = conn.recv(HEADER).decode(FORMAT)
    if not msg:
        return False
    time.sleep(2)

    return True

t = aru.network.SocketServerThread(HOST, PORT, HEADER, FORMAT, sendTime)

t.start()

def signal_handler(signal, frame):
    t.stop = True
    print('set stop')
    t.join()
    print('thread stopped')
    sys.exit(0)

signal.signal(signal.SIGINT, signal_handler)

while True:
    pass