package com.company;

import javax.swing.*;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.net.ServerSocket;
import java.util.Scanner;

public class Main {
    private static ServerSocket serverSocket;
    public static DataOutputStream out;
    public static DataInputStream in;

    public static void sendMessage(String msg){
        try {
            int size = msg.length() + 2;
            out.write((msg + '\0').getBytes(), 0, size);
            System.out.println("SENDING:" + msg + " length:" + size);
        } catch (Exception ex) {
            JOptionPane.showMessageDialog(null,"Send Error: " + ex);
        }
    }

    public static void Listen() {

    }

    public static void main(String[] args) {
        try {
            serverSocket = new ServerSocket(13000);
        } catch (Exception ex) {
            System.out.println("socket failed");
        }
        // read // TODO: Doesnt work yet
        while (true) {
            byte[] data = new byte[1028];
            try {
                //if (serverSocket.getInetAddress().isReachable(1))
                //    System.out.println("New client connected");
                while ((in.read(data, 0, data.length)) != -1) {
                    String msgs = new String(data);
                    System.out.println("received msg:" + msgs);
                    System.out.println(msgs);
                    if (msgs.endsWith("\0")) // check if end of msg (if last char is null)
                    {
                        String[] msg = msgs.split("\0", 10); // split buffer at null terminator(s)
                        for (int t = 0, leng = msg.length; t < leng; t++) {
                            if (msg[t].length() > 0) // if not dead end
                            {
                                System.out.println("RECEIVED:" + msg[t] + '\n');
                                sendMessage(msg[t]);
                            } else
                                break; // data holder can be full of null chars
                        }
                    }
                }
            } catch (Exception ex) {

            }
        }
    }
}
