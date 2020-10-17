using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NetMethods : MonoBehaviour
{
    public InputField Message;
    public Text ChatBox;

    public void BtnClickConnect()
    {
        ConnectToServer(MMONetwork.server, MMONetwork.port);
    }

    public void BtnClickDisconnect()
    {
        ChatBox.text += "Closing connection." + Environment.NewLine;
        Debug.Log("Client socket closing");
        MMONetwork.CloseSocket();
    }

    public void BtnClickSendMsg()
    {
        if (!MMONetwork.socketReady)
        {
            ChatBox.text += "Client socket closed." + Environment.NewLine;
            Debug.Log("Client socket closed.");
            return;
        }

        if (Message.text.Length < 1)
        {
            ChatBox.text += "Type in a message first." + Environment.NewLine;
            Debug.Log("Type in a message first.");
            return;
        }

        SendData(MMONetwork.clientSocket, Message.text);
        Message.text = "";

        Message.Select();
        Message.ActivateInputField();
    }

    public bool ConnectToServer(string hostAdd, int port)
    {
        if (MMONetwork.socketReady)
        {
            ChatBox.text += "Client socket already open." + Environment.NewLine;
            Debug.Log("Client socket already open.");
            return false;
        }

        try
        {
            MMONetwork.conn = new IPEndPoint(IPAddress.Parse(hostAdd), port);
            MMONetwork.clientSocket.BeginConnect(MMONetwork.conn, ConnectCallback, MMONetwork.clientSocket);
            MMONetwork.socketReady = true;
            Debug.Log("Client socket ready: " + MMONetwork.socketReady);
            ChatBox.text += "Client socket ready: " + MMONetwork.socketReady + Environment.NewLine;
        }
        catch (Exception ex)
        {
            Debug.Log("socket error: " + ex.Message);
        }

        return MMONetwork.socketReady;
    }

    public static void SendData(Socket client, string data)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(data + '\0');
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallBack), client);
    }

    static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            int bytesSent = client.EndSend(ar);

            Debug.Log("client sent: " + bytesSent);
        }
        catch (Exception e)
        {
            Debug.Log("error sending message: " + e);
        }
    }

    static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndConnect(ar);
            Debug.Log("Client Socket connected to: " + client.RemoteEndPoint);
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting: " + e);
        }
    }
}
