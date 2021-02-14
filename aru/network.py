import socket
import threading
import queue

class SocketServerThread(threading.Thread):
    def __init__(self, ip="localhost", port=12345, header=64, msgFormat='utf-8', task=None, *args):
        # super(SocketServerThread, self).__init__(*args, **kwargs)
        threading.Thread.__init__(self)
        self.queue = queue.Queue()
        self.ip = ip
        self.port = port
        self.header = header
        self.format = msgFormat

        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.bind((self.ip, self.port))
        self.conn = None
        self.addr = None

        self.task = task
        self.args = args

        self.stop = False

    def run(self):
        self.socket.listen()
        print("Waiting connection...")
        self.conn, self.addr = self.socket.accept() 
        print("Client connected.")

        while not self.stop:
            if not self.task(self.conn, *self.args):
                break
        
        self.socket.close()
        print("Client disconnected.")
    
    def stop(self):
        self.stop = True