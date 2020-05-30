using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System;

public class MenuUX : MonoBehaviour
{
    [SerializeField]
    private InputField ipField, nameField;

    [SerializeField]
    private Text infoText;

    private const string RegularServerIP = "185.163.47.170";

    void Start()
    {
        //START BE FOR TEST PURPOSES
        EstablishClient(RegularServerIP, 52515, "ClientBoi");
    }

    private bool ValidateFormsAndConnect()
    {
        if (nameField.text == "")
        {
            infoText.text = "Invalid name!";
            return false;
        }
        if(nameField.text.Contains(" "))
        {
            infoText.text = "Name cannot contain spaces!";
            return false;
        }
        IPAddress ip;
        if (!IPAddress.TryParse(ipField.text, out ip))
        {
            infoText.text = "Invlaid IP!";
            return false;
        }
        if (ip.AddressFamily != AddressFamily.InterNetwork)
        {
            infoText.text = "Invalid IPv4!";
            return false;
        }

        return EstablishClient(ip.ToString(), Client.port, nameField.text);
    }

    private bool ValidateNameAndConnectRegular()
    {
        if (nameField.text == "")
        {
            infoText.text = "Invalid name!";
            return false;
        }
        if (nameField.text.Contains(" "))
        {
            infoText.text = "Name cannot contain spaces!";
            return false;
        }
        return EstablishClient(RegularServerIP, 52515, nameField.text);
    }

    private bool EstablishClient(string ip, int port, string name) //TODO Add use default host option, with our Moldovian IP
    {
        try
        {
            TcpClient client = new TcpClient(ip.ToString(), 52515);
            Client.SetClient(client, name);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            infoText.text = e.Message;
            return false;
        }
    }

}
