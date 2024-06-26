using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using VContainer;

public struct EventStartBuild
{
    public Entity Item;
    public int InventPos;
}

public class PanelInventory : UIPanel<PanelInventory>, IUIPanel
{
    [SerializeField] private PanelItemList<UIItem> pool;

    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private UIItem equipItem;
    [SerializeField] private GameObject buttonTakeOff;
    [SerializeField] private GameObject buttonEquip;
    [SerializeField] private GameObject buttonBuild;
    [SerializeField] private TextMeshProUGUI buttonBuildText;
    [SerializeField] private GameObject buttonDrop;
    [SerializeField] private GameObject buttonUse;

    [Inject] private UIGame uiGame;
    [Inject] private GameContent content;
    [Inject] private GameCursorSystem cursorSystem;


    public void Init(List<Entity> list, int inventorySize, ComponentUnit unit)
    {
        pool.Init();
        textCount.text = list.Count + "/" + inventorySize;

        if (pool.items.Count != list.Count)
        {
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
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                pool.items[i].Init(content, list[i], i, OnSelectItem);
            }
        }

        if (unit.HandItem != null)
            equipItem.Init(content, unit.HandItem);

        equipItem.gameObject.SetActive(unit.HandItem != null);

        buttonTakeOff.SetActive(unit.HandItem != null);
        
        UpdateButtons();

        if(!IsSelectBuild() && cursorSystem.CursorMode == CursorMode.build)
            cursorSystem.SetCursorMode(CursorMode.normal);
    }

    private void OnSelectItem(int idx)
    {
        var it = pool.items[idx];
        it.Select(!it.IsSelect);

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        var selectItem = GetFirstItem();

        buttonEquip.SetActive(selectItem.Item1 != null && Info.EntityInfo[selectItem.Item1.Item.Id].isEquip);
        buttonDrop.SetActive(IsSelectItem() && uiGame.PanelContainer.gameObject.activeSelf);

        
        bool isBuild = selectItem.Item1 != null && Info.EntityInfo[selectItem.Item1.Item.Id].build != null;
        if (isBuild)
            buttonBuildText.text = Lang.Get("UI", "textBuild") + " [" + Info.EntityInfo[selectItem.Item1.Item.Id].build.cost + "]";
        buttonBuild.SetActive(isBuild);

        buttonUse.SetActive(selectItem.Item1 != null && Info.EntityInfo[selectItem.Item1.Item.Id].isUse);
    }

    public void OnTakeOff()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.index, new MsgClientIndex(MsgClientTypeIndex.unEquip, 0)));
    }

    public void OnEquip()
    {
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i];
            if (it.IsSelect)
            {
                Data.Instance.SendMsg(new MsgClient(MsgClintType.index, new MsgClientIndex(MsgClientTypeIndex.equip, i)));
                return;
            }
        }
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

    private (UIItem, int) GetFirstItem()
    {
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i];
            if (it.IsSelect)
                return (it, i);
        }
        return  (null, 0);
    }

    public void OnBuild()
    {
        var selectItem = GetFirstItem();

        if (selectItem.Item1 != null)
        {
            cursorSystem.SetCursorMode(CursorMode.build);
            Messenger.Send(new EventStartBuild(){Item = selectItem.Item1.Item, InventPos = selectItem.Item2});
        }
    }

    public void OnDrop()
    {
        List<int> list = new List<int>();

        int i = 0;
        foreach (var it in pool.items)
        {
            if (it.IsSelect)
                list.Add(i);
            i++;
        }

        Vector2w pos = uiGame.PanelContainer.Pos;


        if (list.Count > 0)
            Data.Instance.SendMsg(new MsgClient(MsgClintType.moveItems, new MsgClientMoveItems(pos, list, false)));
    }

    public bool IsSelectBuild()
    {
        var selectItem = GetFirstItem();
        bool isBuild = selectItem.Item1 != null && Info.EntityInfo[selectItem.Item1.Item.Id].build != null;
        return isBuild;
    }

    public void OnUse()
    {
        var selectItem = GetFirstItem();

        if (selectItem.Item1 != null && Info.EntityInfo[selectItem.Item1.Item.Id].isUse)
        {
            Data.Instance.SendMsg(new MsgClient(MsgClintType.index, new MsgClientIndex(MsgClientTypeIndex.useItem, selectItem.Item2)));
        }
    }
}
