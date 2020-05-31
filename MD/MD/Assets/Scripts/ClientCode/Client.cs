using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using Assets.Scripts.ClientCode;
using System.Text;
using System.Threading;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{

    #region Singleton

    private static Client singleton;

    void Start()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        GameManager.SubscribeGameSceneLoad(OnGameSceneLoaded);
        singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
            if (tcpClient != null)
            {
                clientThread.Abort();
                tendThread.Abort();
                WriteToServer("MD CLOSE\n");
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
    public const float timeOutTime = 15;

    public static TcpClient tcpClient { get; private set; }
    private static NetworkStream netStream { get; set; }

    private static Thread clientThread;
    private static Thread tendThread;

    private static FlagInterface activeFlagInterface;

    private static string latestOpponentName;
    private static float latestOpponentScore = 0;

    public static void SetClient(TcpClient client, string name)
    {
        if (tcpClient == null)
        {
            tcpClient = client;
        }
        else
        {
            WriteToServer("MD CLOSE\n");
            tcpClient.Close();
            tcpClient = client;
        }
        name = name.Trim(' ');
        Name = name;
        netStream = client.GetStream();
        TimeOutFlag timeOutFlag = new TimeOutFlag();
        FlagInterface flagInterface = new FlagInterface();
        activeFlagInterface = flagInterface;


        singleton.StartCoroutine(singleton.TimeOutRoutine(timeOutFlag));
        singleton.StartCoroutine(singleton.InterfaceRoutine(flagInterface, timeOutFlag));

        clientThread = new Thread(new ParameterizedThreadStart(ClientSideThread));
        clientThread.Start(flagInterface); //WE ARE WORKING BABY!
    }

    #region Game 

    #region Client Side Thread

    private static readonly object _clientBoiBusy = new object();

    private static void ClientSideThread(object flag)
    {
        Debug.Log("Client Side thread has now commenced!");
        FlagInterface flagInterface = (FlagInterface)flag;

        ClientSideThreadServerInitialization(flagInterface);

    }

    private static void ClientSideThreadServerInitialization(FlagInterface flagInterface)
    {
        EnqueueNoTimeoutFlag(flagInterface);
        WriteToServer("MD " + Name + "\n");

        string response = ReadFromServer();
        EnqueueNoTimeoutFlag(flagInterface);
        if (response != "MD OK\n")
        {
            WriteToServer("MD CLOSE\n");
            EnqueueErrorFlag(flagInterface, "Server responded with " + response + " instead of verifying login.");
            return;
        }
        //we got the OK!
        Debug.Log("Established connection to client.");
        //TODO ADD MORE OPTIONS ETC LATER ON RATHER THAN AUTOMATICALLY ENQUEING. ROOM NUMBER ENTERING ETC FOR EXAMPLE OR SONG CHOICE!
        EnqueueQueueMenuActivation(flagInterface);
        WriteToServer("MD ENQUEUE\n");

        tendThread = new Thread(new ParameterizedThreadStart(CallClientSidePerpetualTend));
        tendThread.Start(flagInterface);
        ClientSideThreadPerpetualListen(flagInterface);
    }

    private static void ClientSideThreadPerpetualListen(FlagInterface flagInterface)
    {
        try
        {
            while (tcpClient != null)
            {

                string response = ReadFromServer();

                lock (_clientBoiBusy)
                {
                    response = response.Substring(0, response.Length - 1); //get rid of the \n
                    var fields = response.Split(' ');
                    float valuePassed = 0;
                    string messagePassed = "";
                    if (fields.Length == 3)
                    {
                        if (fields[2][0] == '~')
                        {
                            //this be string msg.
                            fields[2] = fields[2].Remove(0,1);
                            messagePassed = fields[2];
                        }
                        else
                        {
                            //this be float msg.
                            if (!float.TryParse(fields[2], out valuePassed))
                            {
                                Debug.LogError("Invalid response from server: " + response);
                                continue;
                            }
                        }
                    }
                    EnqueueNoTimeoutFlag(flagInterface);
                    string eval = fields[0] + " " + fields[1];
                    switch (eval)
                    {
                        case "MD INVALID":
                            EnqueueErrorFlag(flagInterface, "Server responded with " + response);
                            break;
                        case "MD NO-TIMEOUT":
                            WriteToServer("MD NO-TIMEOUT\n");
                            break;
                        case "MD QUEUE":
                            if(MenuUX.GetSingleton() != null)
                            {
                                EnqueueUpdateEnqueuedNumber(flagInterface, (int)valuePassed);
                            }
                            break;
                        case "MD GAME FDISCONNECT":
                            EnqueueErrorFlag(flagInterface, "Your opponent has lost connection to the server ;_;");
                            break;
                        case "MD GAME-INIT":
                            Debug.Log("Game shall be initalized with player: " + messagePassed);
                            EnqueueGameStartFlag(flagInterface, messagePassed);
                            break;
                        case "MD GAME-FAIL":
                            EnqueueErrorFlag(flagInterface, "You and your opponent were out of tolerable sync and both lost ;_;" );
                            break;
                        case "MD GAME-INDIC":
                            EnqueueUpdateServerPos(flagInterface, valuePassed);
                            break;
                        case "MD GAME-OPP":
                            EnqueueUpdateOpponentPos(flagInterface, valuePassed);
                            break;
                        default:
                            EnqueueErrorFlag(flagInterface, "Unexpected response from server: " + response);
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            EnqueueErrorFlag(flagInterface, e.ToString());
        }
    }

    private static void CallClientSidePerpetualTend(object flagBoi)
    {
        ClientSidePerpetualTend((FlagInterface)flagBoi);
    }

    private static void ClientSidePerpetualTend(FlagInterface flagInterface)
    {
        while (tcpClient != null)
        {
            lock (_clientBoiBusy)
            {
                while (flagInterface.clientThreadFlags.Count != 0)
                {

                    var processed = flagInterface.clientThreadFlags.Dequeue();
                    switch (processed.interfaceMessage)
                    {
                        case ClientThreadMessage.close:
                            WriteToServer("MD CLOSE\n");
                            break;
                        case ClientThreadMessage.ready:
                            WriteToServer("MD GAME READY " + (processed.val).ToString() + "\n");
                            break;
                        case ClientThreadMessage.updateGamePlayerPos:
                            WriteToServer("MD GAME POS " + (processed.val).ToString() + "\n");
                            break;
                    }
                }
            }
        }
    }

    #region Event Subscribers

    private static void OnGameSceneLoaded()
    {
        latestOpponentScore = 0;
        GameManager.GetSingleton().InitializeGameValues(latestOpponentName);
        EnqueueClientThreadGameReadyFlag(activeFlagInterface, SongManager.GetSingleton().GetActiveSong().GetCurrentTempo());
    }

    #endregion

    #endregion

    #region Interface and Timeout

    private IEnumerator InterfaceRoutine(FlagInterface flag, TimeOutFlag timeOutFlag)
    {
        while (tcpClient != null)
        {
            yield return new WaitForEndOfFrame();
            while (flag.gameInterfaceFlags.Count != 0)
            {
                var processed = flag.gameInterfaceFlags.Dequeue();
                switch (processed.interfaceMessage)
                {
                    case InterfaceMessage.resetTimeout:
                        timeOutFlag.resetTimer = true;
                        break;
                    case InterfaceMessage.raiseError:
                        ErrorScene.LoadError(processed.msg);
                        break;
                    case InterfaceMessage.gameStart:
                        SceneManager.LoadScene("Game");
                        latestOpponentName = processed.msg;
                        break;
                    case InterfaceMessage.enableQueueMenu:
                        MenuUX.GetSingleton().RevealQueue();
                        break;
                    case InterfaceMessage.updateEnqueuedNumber:
                        MenuUX.GetSingleton().UpdateEnqueuedNumber((int)processed.val);
                        break;
                    case InterfaceMessage.updateGameServerPos:
                        GameManager.GetSingleton().UpdateServerPos(processed.val);
                        break;
                    case InterfaceMessage.updateGameOpponentPos:
                        GameManager.GetSingleton().UpdateOpponentPos(processed.val);
                        break;
                }
            }
        }
    }

    private IEnumerator TimeOutRoutine(TimeOutFlag flag)
    {
        while (true)
        {
            float time = timeOutTime;
            while (time >= 0)
            {
                yield return new WaitForEndOfFrame();
                time -= Time.deltaTime;
                if (flag.resetTimer)
                {
                    flag.resetTimer = false;
                    time = timeOutTime;
                }
                if (flag.exit)
                {
                    yield break;
                }
            }
            EnqueueClientThreadCloseFlag(activeFlagInterface);
            EnqueueErrorFlag(activeFlagInterface, "Connection to server timed out.");
        }
    }



    #endregion

    #endregion

    #region Client-Specific Utilities

    public static bool WriteToServer(string message)
    {
        try
        {
            if (tcpClient == null)
                return false;
            byte[] data = Utils.GetBytes(message);
            if (displayNetDebug)
            {
                Debug.Log("Writing to server: " + message);
            }
            netStream.Write(data, 0, data.Length);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public static string ReadFromServer()
    {
        try
        {
            byte[] response = new byte[tcpClient.ReceiveBufferSize];
            netStream.Read(response, 0, (int)tcpClient.ReceiveBufferSize);

            string returnData = Encoding.UTF8.GetString(response);
            int indexof = returnData.IndexOf('\0');
            returnData = returnData.Substring(0, indexof == -1 ? returnData.Length : indexof);
            if (displayNetDebug)
            {
                Debug.Log("Server Response: " + returnData);
            }
            return returnData;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return "";
        }
    }

    #endregion

    #region Common Flags

    private static void EnqueueNoTimeoutFlag(FlagInterface flagInterface)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.resetTimeout, 0, ""));
    }
    private static void EnqueueQueueMenuActivation(FlagInterface flagInterface)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.enableQueueMenu, 0, ""));
    }

    private static void EnqueueErrorFlag(FlagInterface flagInterface, string e)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.raiseError, 0, e));
    }

    private static void EnqueueGameStartFlag(FlagInterface flagInterface, string opp)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.gameStart, 0, opp));
    }

    private static void EnqueueUpdateEnqueuedNumber(FlagInterface flagInterface, int num)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.updateEnqueuedNumber, num, ""));
    }

    private static void EnqueueUpdateServerPos(FlagInterface flagInterface, float val)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.updateGameServerPos, val, ""));
    }

    private static void EnqueueUpdateOpponentPos(FlagInterface flagInterface, float val)
    {
        flagInterface.EnqueueInterfaceFlag(new InterfaceDataFlag(InterfaceMessage.updateGameOpponentPos, val, ""));
    }

    #endregion

    #region Common Client Flags

    private static void EnqueueClientThreadCloseFlag(FlagInterface flagInterface)
    {
        flagInterface.EnqueueClientThreadFlag(new ClientThreadFlag(ClientThreadMessage.close, 0, ""));
    }

    private static void EnqueueClientThreadGameReadyFlag(FlagInterface flagInterface, float initTempo)
    {
        flagInterface.EnqueueClientThreadFlag(new ClientThreadFlag(ClientThreadMessage.ready, initTempo, ""));
    }

    private static void EnqueueUpdatePlayerPos(FlagInterface flagInterface, float val)
    {
        flagInterface.EnqueueClientThreadFlag(new ClientThreadFlag(ClientThreadMessage.updateGamePlayerPos, val, ""));
    }

    #endregion

    #region Within Client Requests

    public static void UpdatePlayerPos(float val)
    {
        EnqueueUpdatePlayerPos(activeFlagInterface, val);
    }

    #endregion

    private class TimeOutFlag
    {
        public bool resetTimer = false;
        public bool exit = false;
    }

    private class FlagInterface
    {
        public Queue<InterfaceDataFlag> gameInterfaceFlags = new Queue<InterfaceDataFlag>();
        public Queue<ClientThreadFlag> clientThreadFlags = new Queue<ClientThreadFlag>();
        public void EnqueueInterfaceFlag(InterfaceDataFlag flag)
        {
            gameInterfaceFlags.Enqueue(flag);
        }
        public void EnqueueClientThreadFlag(ClientThreadFlag flag)
        {
            clientThreadFlags.Enqueue(flag);
        }
    }


    private struct InterfaceDataFlag
    {
        public InterfaceMessage interfaceMessage;
        public float val;
        public string msg;

        public InterfaceDataFlag(InterfaceMessage flagMsg, float val, string msg)
        {
            this.interfaceMessage = flagMsg;
            this.val = val;
            this.msg = msg;
        }
    }

    private struct ClientThreadFlag
    {
        public ClientThreadMessage interfaceMessage;
        public float val;
        public string msg;

        public ClientThreadFlag(ClientThreadMessage flagMsg, float val, string msg)
        {
            this.interfaceMessage = flagMsg;
            this.val = val;
            this.msg = msg;
        }
    }

    private enum InterfaceMessage { resetTimeout, raiseError, gameStart, enableQueueMenu, updateEnqueuedNumber, updateGameServerPos, updateGameOpponentPos }
    private enum ClientThreadMessage { close, enqueue, ready, updateGamePlayerPos }
}

