using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsernameDisplay : MonoBehaviour
{
    public Text Text;
    private void Start()
    {
        if (NetworkController.Instance.CurrentUser.Contains("[VERY_SPECIAL_AND_SECRET_STRING]"))
        {
            NetworkController.Instance.CurrentUser = NetworkController.Instance.CurrentUser.Replace("[VERY_SPECIAL_AND_SECRET_STRING]", "");
            Text.text = "Hello, " + NetworkController.Instance.CurrentUser + "! Welcome!";
        }
        else
        {
            Text.text = "Welcome back, " + NetworkController.Instance.CurrentUser + "!";
        }
    }
}
