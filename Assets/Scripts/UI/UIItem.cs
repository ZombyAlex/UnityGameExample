using System;
using System.Collections.Generic;
using SWFServer.Data.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private GameObject panelDurability;
    [SerializeField] private Image durability;

    [SerializeField] private Image back;
    [SerializeField] private List<Sprite> sprites;

    private int index = 0;
    private Action<int> onClick = null;
    private bool isSelect = false;
    private Entity item;
    public bool IsSelect => isSelect;

    public Entity Item => item;


    public void Init(GameContent content, Entity item, int index = 0, Action<int> onClick = null)
    {
        this.item = item;
        this.index = index;
        this.onClick = onClick;
        icon.sprite = content[Info.EntityInfo[item.Id].name];
        if (item.Count != null && item.Count.Value > 1)
        {
            textCount.text = item.Count.Value.ToString();
            textCount.gameObject.SetActive(true);
        }
        else
            textCount.gameObject.SetActive(false);

        panelDurability.SetActive(item.Durability != null);
        if (item.Durability != null)
            durability.fillAmount = item.Durability.Value / Info.EntityInfo[item.Id].durability;

        if(item.Cost!= null && textCost!= null)
            textCost.text = item.Cost.Value.ToString();

        var helper = GetComponent<UIItemHelper>();
        if (helper != null) helper.Init(item);
    }

    public void Select(bool isSelect)
    {
        this.isSelect = isSelect;
        back.sprite = isSelect ? sprites[1] : sprites[0];
    }

    public void OnClick()
    {
        onClick?.Invoke(index);
    }
}
