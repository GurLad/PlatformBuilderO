using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static SocketFunctions;

public class ConnectToServer : MonoBehaviour
{
    public InputField InputField;
    public Text Failed;
    public void Click()
    {
        IPString = HexToIP(InputField.text);
        Debug.Log(IPString);
        Socket socket = Connect();
        if (socket != null)
        {
            socket.SendOne("Hello world!");
            SceneManager.LoadScene("Login");
        }
        else
        {
            Failed.text = "Wrong IP!";
        }
    }
}
