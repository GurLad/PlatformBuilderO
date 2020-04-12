using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static SocketFunctions;
public enum LoginState { Failed, Succeeded, Created }
public class NetworkController : MonoBehaviour
{
    public static NetworkController Instance;
    [HideInInspector]
    public string CurrentUser;
    [HideInInspector]
    public string CurrentLevel;
    [HideInInspector]
    public bool JoinOnline = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
    public LoginState Login(string username, string password)
    {
        Socket sender = Connect();
        sender.SendOne("SEEK_PASSWORD");
        sender.SendOne(username);
        string targetPassword = sender.ReceiveOne();
        targetPassword = targetPassword.Trim();
        Debug.Log(targetPassword);
        if (targetPassword != "")
        {
            if (targetPassword != password)
            {
                Debug.Log(string.Join(",", targetPassword.ToCharArray()));
                Debug.Log(string.Join(",", password.ToCharArray()));
                return LoginState.Failed;
            }
            //sender.CloseSocket();
            CurrentUser = username;
            return LoginState.Succeeded;
        }
        else
        {
            sender = Connect();
            sender.SendOne("SAVE_PASSWORD");
            sender.SendOne(username);
            sender.SendOne(password);
            CurrentUser = "[VERY_SPECIAL_AND_SECRET_STRING]" + username;
            return LoginState.Created;
        }
    }
    public void LoadLevel(string level)
    {
        Socket sender = Connect();
        sender.SendOne("SEEK_LEVEL");
        sender.SendOne(level);
        if (sender.ReceiveOne() != "Nonexistant level")
        {
            GameController.Instance.FromSaveData(sender.ReceiveLargeData());
        }
    }
    private void OnApplicationQuit()
    {
        Socket sender = Connect();
        sender.SendOne("QUIT");
    }
}
