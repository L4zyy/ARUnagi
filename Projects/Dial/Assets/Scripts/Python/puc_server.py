# python unity communication

import socket 
import time
import datetime
import signal
import struct

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
    data = tm.encode(FORMAT)
    # send length of the msg
    conn.send(struct.pack('I', len(data)))
    msg = conn.recv(HEADER).decode(FORMAT)

    if msg == 'true':
        # send msg
        conn.send(data) 
        msg = conn.recv(HEADER).decode(FORMAT)
        if msg == 'true':
            print('msg sending succeed.')
        else:
            print("msg sending failed")
            return False
    else:
        print("msg length sending failed.")
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