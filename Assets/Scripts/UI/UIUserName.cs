using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
public class UIUserName : MonoBehaviour
{
    private TextMeshProUGUI text;
    private uint userId;


    
    void Update()
    {
        string s = Data.Instance.GetUserName(userId);
        
        if (!string.IsNullOrEmpty(s))
        {
            text.text = s;
            enabled = false;
        }
    }

    public void Init(uint userId)
    {
        text = GetComponent<TextMeshProUGUI>();
        this.userId = userId;

        string s = Data.Instance.GetUserName(userId);
        text.text = s;
        enabled = true;
        if (!string.IsNullOrEmpty(s))
            enabled = false;
    }

    public void Off(string s = null)
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = s;
        enabled = false;
    }

    public void SetColor(Color color)
    {
        text.color = color;
    }
}
