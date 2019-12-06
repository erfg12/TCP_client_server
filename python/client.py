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

BUFFER_SIZE = 1024
MESSAGE = 'Hello, World!'.encode()
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


class myThread (threading.Thread):
    def __init__(self, ip, port):
        threading.Thread.__init__(self)
        self.ip = ip
        self.port = port

    def run(self):
        # print('ip:', self.ip)
        s.connect((self.ip, self.port))
        s.send(MESSAGE)

        # listen
        while 1:
            data = s.recv(BUFFER_SIZE)
            if not data:
                break
            # print('received data:', data)
            Client_UI.self.text_output(text=data)
            time.sleep(5)

        s.close()


def connectbtn_callback(instance, ip, port):
    print("connecting...")
    print('ip:', instance)
    print('port:', ip)
    thread1 = myThread(instance, ip)
    thread1.start()


class Client_UI(App):
    def build(self):
        root = FloatLayout()

        layout = BoxLayout(orientation='vertical')
        layout_top = BoxLayout(orientation='horizontal', size_hint=(1, .1))
        layout_bottom = BoxLayout(orientation='horizontal', size_hint=(1, .1))

        text_input = TextInput()
        text_output = TextInput()
        input_username = TextInput(text="user", multiline=False)
        input_ip = TextInput(text="127.0.0.1", multiline=False)
        input_port = TextInput(text="5005", multiline=False)
        connect_button = Button(text="CONNECT")
        connect_button.bind(on_press=partial(
            connectbtn_callback, input_ip.text, input_port.text))
        # connect_button.bind(on_press=connectbtn_callback)
        send_button = Button(text="SEND", size_hint=(.3, 1))

        layout_top.add_widget(Label(text='user'))
        layout_top.add_widget(input_username)
        layout_top.add_widget(Label(text='ip'))
        layout_top.add_widget(input_ip)
        layout_top.add_widget(Label(text='port'))
        layout_top.add_widget(input_port)
        layout_top.add_widget(connect_button)
        layout.add_widget(layout_top)

        layout.add_widget(text_input)

        layout_bottom.add_widget(text_output)
        layout_bottom.add_widget(send_button)
        layout.add_widget(layout_bottom)

        root.add_widget(layout)
        return root


Client_UI().run()
