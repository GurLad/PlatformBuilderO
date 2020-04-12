using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using static SocketFunctions;

public class UploadLevel : MonoBehaviour
{
    public InputField LevelName;
    public Text Logger;
    public void Click()
    {
        Socket sender = Connect();
        sender.SendOne("SAVE_LEVEL");
        sender.SendOne(LevelName.text);
        sender.SendLargeData(NetworkController.Instance.CurrentUser + "~" + GameController.Instance.ToSaveData());
        Logger.text = sender.ReceiveOne();
        Logger.color = new Color(Logger.color.r, Logger.color.g, Logger.color.b, 3);
        //sender.CloseSocket();
    }
    private void Update()
    {
        Logger.color = new Color(Logger.color.r, Logger.color.g, Logger.color.b, Logger.color.a - Time.deltaTime);
    }
}
