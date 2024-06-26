using System;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private int id;
    private Action<int> onClick;

    public void Init(string str)
    {
        text.text = str;
    }

    public void Init(string str, int id, Action<int> onClick)
    {
        text.text = str;
        this.id = id;
        this.onClick = onClick;
    }

    public void OnClick()
    {
        if (onClick != null)
            onClick(id);
    }
}
