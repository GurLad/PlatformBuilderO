using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{
    [Header("Login UI")]
    public InputField UsernameText;
    public InputField PasswordText;
    public Text WrongResultText;
    public void Click()
    {
        switch (NetworkController.Instance.Login(UsernameText.text, PasswordText.text))
        {
            case LoginState.Failed:
                WrongResultText.color = new Color(0.5f, 0, 0);
                WrongResultText.text = "Wrong password!";
                break;
            case LoginState.Succeeded:
                SceneManager.LoadScene("LevelSelect");
                break;
            case LoginState.Created:
                SceneManager.LoadScene("LevelSelect");
                break;
            default:
                break;
        }
    }
}
