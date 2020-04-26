using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    private Text text;
    public void Log(string log)
    {
        text.text = log;
        text.color = new Color(text.color.r, text.color.g, text.color.b, 3);
    }
    private void Start()
    {
        text = GetComponent<Text>();
    }
    private void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime);
    }
}
