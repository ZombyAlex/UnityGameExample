using System;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PanelAddTask : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textSum;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private TextMeshProUGUI textCountMax;
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private TMP_InputField inputFieldCost;
    [SerializeField] private Slider sliderCount;
    [SerializeField] private Slider sliderTime;
    [SerializeField] private GameObject buttonCreate;
    [SerializeField] private GameObject panelInfo;


    private ushort itemId = 0;
    private int cost;
    private int count;
    private int time;

    [Inject] private UIGame uiGame;
    [Inject] private GameContent content;

    void OnEnable()
    {
        var unit = Data.Instance.Unit;
        itemId = 0;
        cost = 0;
        count = 0;
        time = 0;

        buttonCreate.SetActive(false);
        panelInfo.SetActive(false);
    }

    public void OnSelectItem()
    {
        uiGame.ShowPanelSelectItem(OnSelect);
    }

    private void OnSelect(ushort itemId)
    {
        this.itemId = itemId;

        var info = Info.EntityInfo[itemId];

        bool isDone = true;
        int maxCount = int.MaxValue;

        foreach (var item in info.craft.items)
        {
            int count = Util.CalcUnitItems(false, Info.EntityInfo[item.id].id);
            int craftCount = count / item.count;
            if (craftCount < maxCount)
                maxCount = craftCount;

            bool isComplete = count >= item.count;
            if (!isComplete)
                isDone = false;
        }

        icon.sprite = content[info.name];
        textName.text = Lang.Get("items", info.name);


        sliderCount.maxValue = maxCount;
        sliderCount.value = 0;
        count = 0;
        textCount.text = count.ToString();
        textCountMax.text = maxCount.ToString();

        sliderCount.gameObject.SetActive(isDone);

        cost = 0;
        inputFieldCost.text = String.Empty;
        textSum.text = String.Empty;

        sliderTime.maxValue = 48;
        sliderTime.value = 0;

        time = 0;
        textTime.text = Util.ToTimeStringHM(time * 1800);

        buttonCreate.SetActive(isDone);
        panelInfo.SetActive(isDone);
    }

    public void OnCreate()
    {
        if (itemId != 0 && cost > 0 && count > 0 && time > 0)
        {
            GameTask task = new GameTask()
            {
                //Id = GetId(),
                Type = GameTaskType.craft,
                LocId = Data.Instance.LocId,
                Cost = cost,
                //Pos = pos,
                Count = count,
                ItemId = itemId,
                ExecutionTime = time * 1800,
                ReserveMoney = count * count,
                //ReserveItems = list
            };

            Data.Instance.SendMsg(new MsgClient(MsgClintType.task, new MsgClientTask(task)));
            gameObject.SetActive(false);
        }

    }

    public void OnChangeVal()
    {
        if (!int.TryParse(inputFieldCost.text, out cost))
            cost = 0;
        
        count = (int)sliderCount.value;
        time = (int)sliderTime.value;

        textCount.text = count.ToString();

        textTime.text = Util.ToTimeStringHM(time * 1800);

        textSum.text = (cost * count).ToString();
    }

    public bool IsFocus()
    {
        return inputFieldCost.isFocused;
    }
}
