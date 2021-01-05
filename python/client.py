import socket
import time
import threading
from functools import partial
from kivy.app import App
from kivy.uix.button import Button
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.textinput import TextInput
from kivy.uix.label import Label

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


class myThread (threading.Thread):
    def __init__(self, username, ip, port):
        threading.Thread.__init__(self)
        self.ip = ip
        self.port = int(port)
        self.username = username

    def run(self):
        s.connect((self.ip, self.port))
        s.send(('con|' + self.username + '\0').encode())

        while 1:
            data = s.recv(1024)
            if not data:
                continue
            print('Received', repr(data))
            StringData = str(data).replace("\\x00\'", '\n')
            StringData = StringData.replace("b\'", "")
            Client_UI.text_output.text += StringData

        s.close()


class Client_UI(App):
    text_input = TextInput()
    text_output = TextInput()

    def connectbtn_callback(self, value):
        print("connecting...")
        thread1 = myThread(self.input_username.text, self.input_ip.text, self.input_port.text)
        thread1.daemon = True
        thread1.start()

    def sendbtn_callback(self, value):
        print("sending: " + self.text_input.text)
        s.send((self.text_input.text + '\0').encode())
        self.text_input.text = ""
    
    def build(self):
        root = FloatLayout()

        layout = BoxLayout(orientation='vertical')
        layout_top = BoxLayout(orientation='horizontal', size_hint=(1, .1))
        layout_bottom = BoxLayout(orientation='horizontal', size_hint=(1, .1))

        self.input_username = TextInput(text="user", multiline=False)
        self.input_ip = TextInput(text="127.0.0.1", multiline=False)
        self.input_port = TextInput(text="13000", multiline=False)
        connect_button = Button(text="CONNECT")
        connect_button.bind(on_press=self.connectbtn_callback)
        # connect_button.bind(on_press=connectbtn_callback)
        send_button = Button(text="SEND", size_hint=(.3, 1))
        send_button.bind(on_press=self.sendbtn_callback)

        self.text_output.readonly = True

        layout_top.add_widget(Label(text='user'))
        layout_top.add_widget(self.input_username)
        layout_top.add_widget(Label(text='ip'))
        layout_top.add_widget(self.input_ip)
        layout_top.add_widget(Label(text='port'))
        layout_top.add_widget(self.input_port)
        layout_top.add_widget(connect_button)
        layout.add_widget(layout_top)

        layout.add_widget(self.text_output)

        layout_bottom.add_widget(self.text_input)
        layout_bottom.add_widget(send_button)
        layout.add_widget(layout_bottom)

        root.add_widget(layout)
        return root


Client_UI().run()
