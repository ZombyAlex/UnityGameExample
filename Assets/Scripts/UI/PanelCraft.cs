using System;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PanelCraft : UIPanel<PanelCraft>, IUIPanel
{
    [SerializeField] private PanelItemList<UIImageText> pool;

    [SerializeField] private GameObject prefabMaterial;
    [SerializeField] private Transform rootMaterial;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private GameObject buttonCraft;

    private List<GameObject> materials = new List<GameObject>();


    private bool isTask = false;
    private int curCount = 0;
    private ushort itemId;

    private Vector2w posCraft;

    [Inject] private UnitSystem unitSystem;
    [Inject] private GameContent content;

    void Start()
    {
        pool.Init();
        prefabMaterial.SetActive(false);
    }

    public void Init(Vector2w pos)
    {
        posCraft = pos;
        pool.ClearItems();

        materials.ForEach(Destroy);
        materials.Clear();

        WorkbenchType workbench = WorkbenchType.not;

        if (unitSystem.PlayerController != null && unitSystem.PlayerController.State == UnitState.stand)
            workbench = unitSystem.PlayerController.IsWorkbench(pos);

        bool isRent = Data.Instance.LocationType == LocationType.rent1;
        bool isOwner = Data.Instance.LocationOwner == Data.Instance.UserId || isRent;

        List<EntityInfoItem> list = null;

        isTask = false;

        if (!isOwner)
        {
            if (workbench == WorkbenchType.not)
            {
                list = Info.EntityInfo.items.FindAll(f => f.craft != null && f.craft.items.Count > 0
                                                                                               && f.workbenchCraft == workbench);
            }
            else
            {
                var taskId = Data.Instance.Unit.Unit.TaskId;
                if (taskId != 0)
                {
                    var task = Data.Instance.tasks.Find(f => f.Id == taskId);
                    if (task != null && task.ExecutorId == Data.Instance.UserId && task.LocId == Data.Instance.LocId)
                    {
                        list = Info.EntityInfo.items.FindAll(f => f.id == task.ItemId && f.workbenchCraft == workbench);
                        isTask = true;
                    }
                }
            }
        }
        else
        {
            list = Info.EntityInfo.items.FindAll(f => f.craft != null && f.craft.items.Count > 0
                                                                      && f.workbenchCraft == workbench);
        }

        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var go = pool.GetItem();
                var it = go.GetComponent<UIImageText>();
                string txt = Lang.Get("items", list[i].name);
                if (isRent)
                    txt += " [" + list[i].costCraft + "C ]";

                it.Init(content[list[i].name], txt, i, OnClickItem);
                it.ItemId = list[i].id;
                it.Select(false);

                go.SetActive(true);
                pool.items.Add(it);
            }
        }

        textCount.text = String.Empty;
        buttonCraft.SetActive(false);
        slider.gameObject.SetActive(false);
        textCount.gameObject.SetActive(false);
    }

    private void OnClickItem(int index)
    {
        ushort itemId = 0;
        for (int i = 0; i < pool.items.Count; i++)
        {
            var it = pool.items[i].GetComponent<UIImageText>();
            it.Select(i == index);
            if (i == index)
                itemId = it.ItemId;
        }

        ShowMaterials(itemId);
    }

    private void ShowMaterials(ushort itemId)
    {
        this.itemId = itemId;
        materials.ForEach(Destroy);
        materials.Clear();

        var info = Info.EntityInfo[itemId];

        bool isDone = true;
        int maxCount = int.MaxValue;

        foreach (var item in info.craft.items)
        {
            var go = Instantiate(prefabMaterial, rootMaterial);

            int count = Util.CalcUnitItems(isTask, Info.EntityInfo[item.id].id);
            int craftCount = count / item.count;
            if (craftCount < maxCount)
                maxCount = craftCount;

            bool isComplete = count >= item.count;
            if (!isComplete)
                isDone = false;

            var it = go.GetComponent<UIImageTexts>();
            it.Init(content[item.id],
                new List<string>() { Lang.Get("items", item.id), count + "/" + item.count });
            it.TextColor(1, isComplete ? Color.green : Color.white);

            materials.Add(go);
            go.SetActive(true);
        }

        slider.maxValue = maxCount;
        slider.value = 0;
        curCount = 0;
        textCount.text = curCount.ToString();
        slider.gameObject.SetActive(isDone);
        buttonCraft.SetActive(isDone);
        textCount.gameObject.SetActive(isDone);
    }

    

    public void OnChangeSlider()
    {
        int n = (int)slider.value;
        curCount = n;
        textCount.text = curCount.ToString();
    }

    public void OnCraft()
    {
        if (curCount > 0)
        {
            Data.Instance.SendMsg(new MsgClient(MsgClintType.craft, new MsgClientCraft(itemId, curCount, posCraft)));
            gameObject.SetActive(false);
        }
    }
}
