using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public string Name;
    public Text Text;
    [HideInInspector]
    public bool JoinOnline;
    public void Load()
    {
        NetworkController.Instance.CurrentLevel = Name;
        NetworkController.Instance.JoinOnline = JoinOnline;
        SceneManager.LoadScene("Play");
    }
    public void SetLevel(string level)
    {
        Name = level;
        Text.text = level;
    }
}
