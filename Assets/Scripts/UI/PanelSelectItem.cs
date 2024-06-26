using System;
using SWFServer.Data.Entities;
using UnityEngine;
using VContainer;

public class PanelSelectItem : UIPanel<PanelSelectItem>, IUIPanel
{
    [SerializeField] private PanelItemList<UIImageText> pool;

    private ushort itemId = 0;
    private Action<ushort> onSelect;

    [Inject] private GameContent content;
    public void Init(Action<ushort> onSelect)
    {
        pool.Init();
        this.onSelect = onSelect;
        pool.ClearItems();
        itemId = 0;
        int i = 0;
        foreach (var it in Info.EntityInfo.items)
        {
            if (it.craft != null)
            {
                var go = pool.GetItem();
                var item = go.GetComponent<UIImageText>();
                item.ItemId = it.id;
                item.Init(content[it.name], Lang.Get("items", it.name), i, OnSelectItem);
                go.SetActive(true);
                pool.items.Add(item);
                i++;
            }
        }
    }

    private void OnSelectItem(int idx)
    {
        for (int i = 0; i < pool.items.Count; i++)
        {
            pool.items[i].Select(i == idx);
        }

        itemId = pool.items[idx].ItemId;
    }

    public void OnSelect()
    {
        if (itemId != 0)
        {
            onSelect.Invoke(itemId);
            gameObject.SetActive(false);
        }
    }
}
