using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Assets.Scripts.ClientCode;
using System.Text;
using System.Threading;

public class Client: MonoBehaviour
{

    #region Singleton

    private static Client singleton;

    void Start()
    {
        if(singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if(singleton == this)
        {
            singleton = null;
            if (tcpClient != null)
            {
                clientThread.Abort();
                WriteToServer("MD CLOSE");
                netStream.Close();
                tcpClient = null;
                netStream = null;
            }
        }
    }

    public static Client GetSingleton()
    {
        return singleton;
    }

    #endregion


    public static bool displayNetDebug = true;

    public static string Name { get; private set; }

    public const int port = 52515;
    public static TcpClient tcpClient { get; private set; }
    private static NetworkStream netStream { get; set; }

    private static Thread clientThread;

    public static void SetClient(TcpClient client, string name)
    {
        if (tcpClient == null)
        {
            tcpClient = client;
        }
        else
        {
            WriteToServer("MD CLOSE");
            tcpClient.Close();
            tcpClient = client;
        }
        name = name.Trim(' ');
        Name = name;
        netStream = client.GetStream();
        TimeOutFlag timeOutFlag = new TimeOutFlag();
        InterfaceFlag interfaceFlag = new InterfaceFlag();

        clientThread = new Thread(new ParameterizedThreadStart(ClientSideThread));
        clientThread.Start(interfaceFlag); //WE ARE WORKING BABY!
    }

    #region Game 

    private static void ClientSideThread(object flag)
    {
        Debug.Log("Client Side thread has now commenced!");
        InterfaceFlag interfaceFlag = (InterfaceFlag)flag;

        WriteToServer("MD " + Name);

    }

    private IEnumerator TimeOutRoutine(TimeOutFlag flag)
    {

    }

    private IEnumerator InterfaceRoutine(InterfaceFlag flag, TimeOutFlag timeOutFlag)
    {

    }

    #endregion

    #region Client-Specific Utilities

    public static bool WriteToServer(string message)
    {
        if (tcpClient == null)
            return false;
        byte[] data = Utils.GetBytes(message);
        if (displayNetDebug)
        {
            Debug.Log("Writing to server: " + message);
        }
        tcpClient.GetStream().Write(data, 0, data.Length);
        return true;
    }

    public static string ReadFromServer()
    {
        byte[] response = new byte[tcpClient.ReceiveBufferSize];
        netStream.Read(response, 0, (int)tcpClient.ReceiveBufferSize);

        string returnData = Encoding.UTF8.GetString(response);
        if (displayNetDebug)
        {
            Debug.Log("Server Response: " + returnData);
        }
        return returnData;
    }


    #endregion

    private class TimeOutFlag
    {
        public bool resetTimer = false;
    }

    private class InterfaceFlag
    {
        
    }

}

