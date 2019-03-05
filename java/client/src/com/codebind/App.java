package com.codebind;

import javax.swing.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.*;
import java.net.*;
import java.util.Arrays;

public class App {
    private JPanel mainPanel;
    private JTextField nickBox;
    private JTextArea txtArea;
    private JTextField hostBox;
    private JTextField portBox;
    private JButton connectButton;
    private JTextField txtMsg;
    private JButton sendButton;
    private JButton memsButton;

    private Socket clientSocket;
    private DataOutputStream out;
    private DataInputStream in;

    public class ListenThread extends Thread {

        public void run(){
            byte[] data = new byte[1028];
            try {
                while ((in.read(data, 0, data.length)) != -1) {
                    String msgs = new String(data);
                    System.out.println(msgs);
                    if (msgs.endsWith("\0")) // check if end of msg (if last char is null)
                    {
                        String[] msg = msgs.split("\0", 10); // split buffer at null terminator(s)
                        for (int t = 0, leng = msg.length; t < leng; t++)
                        {
                            if (msg[t].length() > 0) // if not dead end
                            {
                                txtArea.append(msg[t] + '\n');
                            }
                            else
                                break; // data holder can be full of null chars
                        }
                    }
                }
            } catch (Exception ex){

            }
        }
    }

    public App() {
        connectButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                if (connectButton.getText() == "disconnect"){
                    stopConnection();
                    return;
                }
                txtArea.append("Attempting to connect...\n");
                try {
                    clientSocket = new Socket(hostBox.getText(), Integer.parseInt(portBox.getText()));
                } catch (Exception ex){
                    JOptionPane.showMessageDialog(null,"Connection Error: " + ex);
                    return;
                }

                try {
                    out = new DataOutputStream(clientSocket.getOutputStream());
                    in = new DataInputStream(clientSocket.getInputStream());
                } catch (Exception ex) {
                    JOptionPane.showMessageDialog(null,"Write Error: " + ex);
                    return;
                }
                txtArea.append("Connected to server!\n");
                connectButton.setText("disconnect");
                ListenThread startThread = new ListenThread();
                startThread.start();
            }
        });
        sendButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                sendMessage(txtMsg.getText());
                txtMsg.setText("");
            }
        });
    }

    public void sendMessage(String msg) {
        try {
            int size = nickBox.getText().length() + msg.length() + 2;
            out.write((nickBox.getText() + ":" + msg + '\0').getBytes(), 0, size);
            System.out.println("SENDING:" + msg + " length:" + size);
        } catch (Exception ex) {
            JOptionPane.showMessageDialog(null,"Send Error: " + ex);
        }
    }

    public void stopConnection() {
        try {
            txtArea.append("Disconnected from server\n");
            connectButton.setText("connect");
            in.close();
            out.close();
            clientSocket.close();
        } catch (Exception ex){

        }
    }

    public static void main(String[] args) {
        JFrame frame = new JFrame("TCP Client");
        frame.setContentPane(new App().mainPanel);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.pack();
        frame.setVisible(true);
    }
}