using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public string Name;
    public Text Text;
    public void Load()
    {
        NetworkController.Instance.CurrentLevel = Name;
        SceneManager.LoadScene("Play");
    }
    public void SetLevel(string level)
    {
        Name = level;
        Text.text = level;
    }
}
