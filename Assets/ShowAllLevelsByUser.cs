using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static SocketFunctions;

public class ShowAllLevelsByUser : MonoBehaviour
{
    public LoadLevel Base;
    private void Start()
    {
        Socket sender = Connect();
        sender.SendOne("SEEK_LEVELS_BY");
        sender.SendOne(NetworkController.Instance.CurrentUser);
        string[] allLevels = sender.ReceiveOne().Split(';');
        for (int i = 0; i < allLevels.Length; i++)
        {
            LoadLevel newButton = Instantiate(Base.gameObject, Base.transform.parent).GetComponent<LoadLevel>();
            newButton.SetLevel(allLevels[i]);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * i);
            newButton.gameObject.SetActive(true);
        }
    }
}
