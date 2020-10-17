using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// network variables stored here to share amongst other functions
/// </summary>
public static class MMONetwork
{
    public static bool scrolling = false; // not part of networking

    public static string clientName;
    public static bool socketReady;
    public static string response;
    public static byte[] clientBuffer = new byte[1024];

    //creating the socket TCP
    public static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    //declare end point
    public static IPEndPoint conn;

    public static string server = "127.0.0.1"; // hostname or ip
    public static int port = 13000;

    public static void CloseSocket()
    {
        if (!socketReady)
        {
            return;
        }

        clientSocket.Close();
        socketReady = false;
    }
}
