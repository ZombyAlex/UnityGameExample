using System.Collections;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITask : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textId;
    [SerializeField] private TextMeshProUGUI textType;
    [SerializeField] private UIUserName textCustomer;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private TextMeshProUGUI textCoord;
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private UIUserName textExecutor;
    [SerializeField] private GameObject buttonTake;
    [SerializeField] private GameObject buttonCancel;
    [SerializeField] private GameObject buttonRemove;
    [SerializeField] private GameObject buttonPerform;

    private GameTask task;

    void Update()
    {
        var t = task.ExecutionTime - Data.Instance.ServerTime;
        textTime.text = Util.ToTimeStringHMS((int)t);
    }

    public void Init(GameTask task, GameContent content)
    {
        this.task = task;

        textId.text = "#" + task.Id;
        textType.text = Lang.Get("UI", "t_" + task.Type);

        if (task.UserId != 0)
            textCustomer.Init(task.UserId);
        else
            textCustomer.Off("Sigma");

        icon.sprite = content[Info.EntityInfo[task.ItemId].name];

        textCount.text = task.Count.ToString();
        textCost.text = task.Cost + " [" + (task.Cost * task.Count) + "]";
        textCoord.text = "X=" + task.Pos.x + " Y=" + task.Pos.y;
        var t = task.ExecutionTime - Data.Instance.ServerTime;
        textTime.text = Util.ToTimeStringHMS((int)t);

        if (task.ExecutorId != 0)
        {
            textExecutor.Init(task.ExecutorId);
            textExecutor.SetColor(task.ExecutorId == Data.Instance.UserId ? Color.green : Color.white);
        }
        else
        {
            textExecutor.Off("---");
            textExecutor.SetColor(Color.white);
        }

        buttonTake.SetActive(task.ExecutorId == 0);
        buttonCancel.SetActive(task.ExecutorId == Data.Instance.UserId);
        buttonRemove.SetActive(task.UserId == Data.Instance.UserId);
        buttonPerform.SetActive(task.ExecutorId == Data.Instance.UserId && task.LocId == Data.Instance.LocId);
    }

    public void OnTake()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.getTask, new MsgClientGetTask(task.Id)));
    }

    public void OnCancel()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.cancelTask)));
    }

    public void OnRemove()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.id, new MsgClientId(task.Id, MsgClientTypeId.removeTask)));
    }

    public void OnPerform()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.id, new MsgClientId(task.Id, MsgClientTypeId.performTask)));
    }
}
