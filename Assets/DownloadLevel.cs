using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using static SocketFunctions;

public class DownloadLevel : MonoBehaviour
{
    public InputField LevelName;
    public void Click()
    {
        NetworkController.Instance.LoadLevel(LevelName.text);
    }
}
