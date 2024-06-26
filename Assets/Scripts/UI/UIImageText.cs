using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIImageText : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Image back;
    [SerializeField] private List<Sprite> sprites;

    private int index = 0;
    private Action<int> onClick = null;

    public ushort ItemId { get; set; }

    public void Init(Sprite sprite, string txt, int index = 0, Action<int> onClick = null)
    {
        this.onClick = onClick;
        this.index = index;

        image.sprite = sprite;
        text.text = txt;
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
