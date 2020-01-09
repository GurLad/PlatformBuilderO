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
        Socket sender = Connect();
        sender.SendOne("SEEK_LEVEL");
        sender.SendOne(LevelName.text);
        if (sender.RecieveOne() != "Nonexistant level")
        {
            GameController.Instance.FromSaveData(sender.ReceiveLargeData());
        }
    }
}
