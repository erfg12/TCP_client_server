using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class NetListen : MonoBehaviour
{
    public Text ChatBox;
    public static string response = "";

    // Update is called once per frame
    void Update()
    {
        if (MMONetwork.socketReady)
        {
            CheckForData(MMONetwork.clientSocket);
            if (response.Length > 1)
            {
                Debug.Log("data from server: \"" + response + "\"");
                ChatBox.text += response;
                response = "";
            }
        }
    }

    public static void CheckForData(Socket client)
    {
        try
        {
            client.BeginReceive(MMONetwork.clientBuffer, 0, MMONetwork.clientBuffer.Length, 0, new AsyncCallback(ReceiveCallback), client);
        }
        catch (Exception e)
        {
            Debug.Log("error receiving the data: " + e.Message);
        }
    }

    static void ReceiveCallback(IAsyncResult ar)
    {
        if (!MMONetwork.socketReady)
        {
            return;
        }

        try
        {
            // Read data from the remote device.
            Socket client = (Socket)ar.AsyncState;
            int bytesRead = client.EndReceive(ar);

            if (bytesRead == 0)
            {
                Debug.Log("no more data to receive");
                return;
            }

            var data = new byte[bytesRead];
            Array.Copy(MMONetwork.clientBuffer, data, bytesRead);

            response = Encoding.Default.GetString(MMONetwork.clientBuffer).TrimEnd('\0') + Environment.NewLine;

            Array.Clear(MMONetwork.clientBuffer, 0, MMONetwork.clientBuffer.Length);
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        MMONetwork.CloseSocket();
    }

    void OnDisable()
    {
        MMONetwork.CloseSocket();
    }
}
