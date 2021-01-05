import socket
import time
import threading

TCP_IP = '127.0.0.1'
TCP_PORT = 13000

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

class myThread(threading.Thread):
    def __init__(self):
        print("server has started")
        threading.Thread.__init__(self)
        conn, addr = s.accept()
        
        print('Connection address:', addr)
    
    def Command(self, value):
        return value

    def run(self):
        print("Server listening")
        while 1:
            data = conn.recv(1024)
            if not data:
                continue
            StringData = str(data).replace("\\x00\'", '\n')
            StringData = StringData.replace("b\'", "")
            print('received data:', StringData)
            if '|' in StringData:
                Command(StringData)
            conn.send(data)
        conn.close()

class Server():
    def run(self):
        s.bind((TCP_IP, TCP_PORT))
        s.listen(50)

        thread1 = myThread()
        thread1.start()

Server().run()