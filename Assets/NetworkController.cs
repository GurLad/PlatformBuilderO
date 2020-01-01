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
    [Header("Login UI")]
    public InputField UsernameText;
    public InputField PasswordText;
    public Text ResultText;
    public void LoginButton()
    {
        switch (Login(UsernameText.text, PasswordText.text))
        {
            case LoginState.Failed:
                ResultText.color = Color.red;
                ResultText.text = "Wrong password!";
                break;
            case LoginState.Succeeded:
                ResultText.color = Color.green;
                ResultText.text = "Welcome back, " + UsernameText.text + "!";
                break;
            case LoginState.Created:
                ResultText.color = Color.green;
                ResultText.text = "Hello, " + UsernameText.text + "! Welcome!";
                break;
            default:
                break;
        }
    }
    public LoginState Login(string username, string password)
    {
        Socket sender = Connect();
        sender.SendOne("SEEK_PASSWORD");
        sender.SendOne(username);
        string targetPassword = sender.RecieveOne();
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
            sender.CloseSocket();
            return LoginState.Succeeded;
        }
        else
        {
            sender = Connect();
            sender.SendOne("SAVE_PASSWORD");
            sender.SendOne(username);
            sender.SendOne(password);
            return LoginState.Created;
        }
    }
}
