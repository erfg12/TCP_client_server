using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IF_Processing : MonoBehaviour
{
    private InputField IF;
    public Text ChatBox;

    // Start is called before the first frame update
    void Start()
    {
        IF = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return)) {
            if (!MMONetwork.socketReady)
            {
                ChatBox.text += "Client socket closed." + Environment.NewLine;
                Debug.Log("Client socket closed.");
                return;
            }

            if (IF.text.Length < 1)
            {
                ChatBox.text += "Type in a message first." + Environment.NewLine;
                Debug.Log("Type in a message first.");
                return;
            }

            NetMethods.SendData(MMONetwork.clientSocket, IF.text);
            IF.text = "";

            IF.Select();
            IF.ActivateInputField();
        }
    }
}
