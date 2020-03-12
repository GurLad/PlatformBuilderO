using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static SocketFunctions;

public class ShowAllOnlineLevels : MonoBehaviour
{
    public LoadLevel Base;
    private void Start()
    {
        Socket sender = Connect();
        sender.SendOne("SEEK_ONLINE_LEVELS");
        string[] allLevels = sender.RecieveOne().Split(';');
        for (int i = 0; i < allLevels.Length - 1; i++)
        {
            LoadLevel newButton = Instantiate(Base.gameObject, Base.transform.parent).GetComponent<LoadLevel>();
            newButton.SetLevel(allLevels[i]);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * i);
            newButton.JoinOnline = true;
            newButton.gameObject.SetActive(true);
        }
    }
}
