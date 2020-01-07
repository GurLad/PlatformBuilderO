using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [Header("Login UI")]
    public InputField UsernameText;
    public InputField PasswordText;
    public Text WrongResultText;
    public Text FinalResultText;
    public GameObject LoginButton;
    public void Click()
    {
        switch (NetworkController.Instance.Login(UsernameText.text, PasswordText.text))
        {
            case LoginState.Failed:
                WrongResultText.color = new Color(0.5f, 0, 0);
                WrongResultText.text = "Wrong password!";
                break;
            case LoginState.Succeeded:
                FinalResultText.color = new Color(0, 0.5f, 0);
                FinalResultText.text = "Welcome back, " + UsernameText.text + "!";
                gameObject.SetActive(false);
                LoginButton.SetActive(true);
                break;
            case LoginState.Created:
                FinalResultText.color = new Color(0, 0.5f, 0);
                FinalResultText.text = "Hello, " + UsernameText.text + "! Welcome!";
                gameObject.SetActive(false);
                LoginButton.SetActive(true);
                break;
            default:
                break;
        }
    }
}
