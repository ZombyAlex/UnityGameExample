using SWFServer.Data.Entities;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using VContainer;

public class PanelContainer : UIPanel<PanelContainer>, IUIPanel
{
    [SerializeField] private PanelItemList<UIItem> pool;

    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private GameObject buttonGet;

    private Vector2w pos;
    public Vector2w Pos => pos;

    [Inject] private GameContent content;

    public void Init(List<Entity> list, int inventorySize, Vector2w pos)
    {
        pool.Init();
        this.pos = pos;
        textCount.text = list.Count + "/" + inventorySize;


        pool.ClearItems();

        int idx = 0;
        foreach (var it in list)
        {
            var go = pool.GetItem();
            var item = go.GetComponent<UIItem>();
            item.Init(content, it, idx, OnSelectItem);
            item.Select(false);
            go.transform.SetAsLastSibling();
            go.SetActive(true);
            pool.items.Add(item);
            idx++;
        }


        UpdateButtons();
    }

    private void UpdateButtons()
    {
        buttonGet.SetActive(IsSelectItem());
    }

    private bool IsSelectItem()
    {
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i];
            if (it.IsSelect)
                return true;
        }

        return false;
    }

    private void OnSelectItem(int idx)
    {
        var it = pool.items[idx];
        it.Select(!it.IsSelect);

        UpdateButtons();
    }

    public void OnGet()
    {
        List<int> list = new List<int>();

        int i = 0;
        foreach (var it in pool.items)
        {
            if (it.IsSelect)
                list.Add(i);
            i++;
        }

        if (list.Count > 0)
            Data.Instance.SendMsg(new MsgClient(MsgClintType.moveItems, new MsgClientMoveItems(pos, list, true)));
    }
}
