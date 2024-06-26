using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIImageTexts : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private List<TextMeshProUGUI> texts;

    public void Init(Sprite sprite, List<string> txt)
    {
        image.sprite = sprite;
        for (int i = 0; i < txt.Count; i++)
        {
            texts[i].text = txt[i];
        }
    }

    public void TextColor(int index, Color color)
    {
        texts[index].color = color;
    }
}
