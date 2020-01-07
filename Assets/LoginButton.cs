using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginButton : MonoBehaviour
{
    public GameObject LoginPanel;
    public void Click()
    {
        LoginPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
