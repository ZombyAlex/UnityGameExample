using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private Image back;
    [SerializeField] private List<Sprite> sprites;

    private int index = 0;
    private Action<int> onClick = null;

    public void Init(Sprite sprite, int index = 0, Action<int> onClick = null)
    {
        image.sprite = sprite;
    }


    public void Select(bool isSelect)
    {
        back.sprite = isSelect ? sprites[1] : sprites[0];
    }

    public void OnClick()
    {
        onClick?.Invoke(index);
    }
}
