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

tm = {'data': None}

def sendTime(conn, data_info):
    data = data_info['data']
    if data == None:
        return True

    bts = data.encode(FORMAT)
    # send length of the msg
    conn.send(struct.pack('I', len(bts)))
    msg = conn.recv(HEADER).decode(FORMAT)

    if msg == 'true':
        # send msg
        conn.send(bts)
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

t = aru.network.SocketServerThread(HOST, PORT, HEADER, FORMAT, sendTime, tm)

t.start()

time.sleep(15)
tm['data'] = datetime.datetime.now().strftime("%X")
print(tm['data'])

time.sleep(15)
tm['data'] = datetime.datetime.now().strftime("%X")
print(tm['data'])

def signal_handler(signal, frame):
    t.stop = True
    print('set stop')
    t.join()
    print('thread stopped')
    sys.exit(0)

signal.signal(signal.SIGINT, signal_handler)

while True:
    pass