using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using SWFServer.Data;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PanelBuy : UIPanel<PanelBuy>, IUIPanel
{
    [SerializeField] private PanelItemList<UIItem> pool;
    
    [SerializeField] private GameObject buttonBuy;

    private Vector2w pos;
    public Vector2w Pos => pos;

    private List<Entity> list;

    [Inject] private GameContent content;

    public void Init(List<Entity> list, Vector2w pos)
    {
        this.list = list;
        pool.Init();
        this.pos = pos;

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
        buttonBuy.SetActive(IsSelectItem());
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
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i];
            it.Select(i == idx);
        }

        UpdateButtons();
    }

    public void OnBuy()
    {
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i];
            if (it.IsSelect)
            {
                var item = list[i];
                Data.Instance.SendMsg(new MsgClient(MsgClintType.buy, new MsgClientBuy(pos, i, item.Id, item.Count.Value)));
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
