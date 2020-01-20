using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTile : MonoBehaviour
{
    public int ID;
    public void Click()
    {
        GameController.Instance.SelectTile(ID);
    }
}
