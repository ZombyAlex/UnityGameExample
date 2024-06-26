using System;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[Serializable]
public class CursorSprite
{
    public CursorMode Mode;
    public Sprite Sprite;
}

public struct EventUpdateCursor{}

public struct EventOnCraft
{
    public Vector2w pos;
}


public class UIGame : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI textUid;
    [SerializeField] private TextMeshProUGUI textMapId;
    [SerializeField] private TextMeshProUGUI textDate;
    [SerializeField] private TextMeshProUGUI textMoney;
    [SerializeField] private TextMeshProUGUI textCoord;

    [SerializeField] private UIProgress satietyProgress;
    [SerializeField] private UIProgress saturationProgress;

    [SerializeField] private Transform panelLabel;
    [SerializeField] private GameObject prefabUserName;

    [SerializeField] private TextMeshProUGUI textMainMessage;
    [SerializeField] private Image cursorImage;
    [SerializeField] private GameObject respawnSign;
    [SerializeField] private GameObject textBuildZone;

    [Header("Buttons")] 
    [SerializeField] private GameObject buttonCreateUnit;
    [SerializeField] private GameObject buttonSleep;
    [SerializeField] private GameObject buttonDoor;
    

    [Header("Panels")]
    [SerializeField] private PanelChat panelChat;
    //[SerializeField] private PopupInfo popupInfo;
    [SerializeField] private PopupInfo popupInfoResource;
    [SerializeField] private PanelMap panelMap;
    [SerializeField] private PanelMenu panelMenu;
    [SerializeField] private PanelYesNo panelYesNo;
    [SerializeField] private PanelUserAction panelUserAction;
    [SerializeField] private PanelRating panelRating;
    //[SerializeField] private PanelInventory panelInventory;
    [SerializeField] private PanelUnitAction panelUnitAction;
    [SerializeField] private PanelTasks panelTasks;
    [SerializeField] private PanelCraft panelCraft;
    [SerializeField] private PanelContainer panelContainer;
    [SerializeField] private PanelSkills panelSkills;
    [SerializeField] private PanelAddTask panelAddTask;
    //[SerializeField] private PanelSelectItem panelSelectItem;


    [Header("Commons")] 
    [SerializeField] private GameObject flyText;
    [SerializeField] private Transform flyTextRoot;

    [SerializeField] private GameObject durabilityBar;
    [SerializeField] private Transform durabilityBarRoot;
    [SerializeField] private List<CursorSprite> cursorSprites;

    //public UIProgress SatietyProgress => satietyProgress;
    //public UIProgress PollutionProgress => pollutionProgress;
    
    public PanelMap PanelMap => panelMap;

    public PanelContainer PanelContainer => panelContainer;

    //public PanelInventory PanelInventory => panelInventory;

    private Dictionary<uint, FlyTextValue> flyTextValues = new Dictionary<uint, FlyTextValue>();

    private UnitSystem unitSystem;
    private MapPosSystem mapPosSystem;
    private MapSystem mapSystem;
    private GameCursorSystem cursorSystem;

    [Inject]
    public void Constuct(MapPosSystem mapPosSystem, MapSystem mapSystem, GameCursorSystem cursorSystem)
    {
        this.mapPosSystem = mapPosSystem;
        this.mapSystem = mapSystem;
        this.cursorSystem = cursorSystem;
    }

    void Awake()
    {
        cursorImage.gameObject.SetActive(false);

        panelChat.Init();

        Messenger.AddListener<EventUpdateCursor>(OnUpdateCursor);
        Messenger.AddListener<EventOnCraft>(OnCraft);
    }

    public void Init(UnitSystem unitSystem)
    {
        this.unitSystem = unitSystem;
    }

    void OnDestroy()
    {
        Messenger.RemoveListener<EventUpdateCursor>(OnUpdateCursor);
        Messenger.RemoveListener<EventOnCraft>(OnCraft);
    }

    private void OnCraft(EventOnCraft obj)
    {
        OnCraft(obj.pos);
    }

    private void OnUpdateCursor(EventUpdateCursor obj)
    {
        UpdateCursor();
    }

    void Update()
    {
        textDate.text = GetDateString();

        if (!IsFocus())
        {
            if (Input.GetKeyDown(KeyCode.C)) OnChat();

        }

        foreach (var it in flyTextValues)
        {
            if (it.Value == null)
            {
                flyTextValues.Remove(it.Key);
                break;
            }
        }
        
        if (mapSystem.IsMap)
        {
            Vector2w pos = mapPosSystem.MapPos;
            textCoord.text = "X=" + pos.x + "  Y=" + pos.y;

            bool isBuildZone = false;
            GameConst.buildPlaces.ForEach(f =>
            {
                if(f.Contains(pos))
                    isBuildZone = true;
            });

            if (textBuildZone.activeSelf != isBuildZone) textBuildZone.SetActive(isBuildZone);
        }
        else
        {
            textCoord.text = String.Empty;
            if(textBuildZone.activeSelf) textBuildZone.SetActive(false);
        }

        bool isUnit = unitSystem.PlayerTransform != null;
        if (buttonCreateUnit.activeSelf == isUnit)
            buttonCreateUnit.SetActive(!isUnit);

        bool isSleep = unitSystem.PlayerController != null && unitSystem.PlayerController.State == UnitState.stand 
                                                                     && unitSystem.PlayerController.IsSleepPos();
        if (buttonSleep.activeSelf != isSleep)
            buttonSleep.SetActive(isSleep);


        bool isDoor = unitSystem.PlayerController != null && unitSystem.PlayerController.State == UnitState.stand
                                                          && unitSystem.PlayerController.IsDoor();

        if (buttonDoor.activeSelf != isDoor)
            buttonDoor.SetActive(isDoor);

        if (panelContainer.gameObject.activeSelf && unitSystem.PlayerController.IsMove())
            panelContainer.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (cursorSystem.CursorMode != CursorMode.normal)
            UpdateCursorPos();
    }

    private void UpdateCursorPos()
    {
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorImage.rectTransform.position = Input.mousePosition + new Vector3(24, -24);
    }

    public bool IsFocus()
    {
        return panelChat.IsFocus() || panelAddTask.IsFocus();
    }

    public bool IsFocusChat()
    {
        return panelChat.IsFocus();
    }

    private string GetDateString()
    {
        long val = (long)Data.Instance.ServerTime;

        long year = (long)(val / GameConst.year);
        long mount = (val % (long)GameConst.year) / (long)GameConst.month + 1;
        long day = (val % (long)GameConst.month) / GameConst.day + 1;
        long hour = (val % GameConst.day) / GameConst.hour;
        long min = (long)((val % GameConst.hour) / GameConst.minute);

        return "<color=#FF9400FF>Year:</color> " + year + "  <color=#FF9400FF>Month:</color> " + mount +
               "  <color=#FF9400FF>Day:</color> " + day + "  <color=#FF9400FF>Time:</color> " + hour + ":" + min.ToString("00");

    }

    public void UpdateUID()
    {
        textUid.text = "UID: " + Data.Instance.UserId;
        textMapId.text = "Map: " + Data.Instance.LocId;
    }

    public void UpdateMoney()
    {
        if (Data.Instance.Money > 0)
            textMoney.text = Data.Instance.Money.ToString("###.###.###.###");
        else
            textMoney.text = Data.Instance.Money.ToString();
    }

    public void UpdateUserList(List<uint> users)
    {
        panelChat.UpdateUserList(users);
    }

    public void SelectUserChat(uint userId)
    {
        panelChat.SelectUserChat(userId);
    }

    public void ChatMsg(MsgServerChat msg)
    {
        panelChat.AddText(msg.isChannel, msg.channelId, msg.name, msg.text);
    }

    public void OnChat()
    {
        if (panelChat.gameObject.activeSelf)
            panelChat.gameObject.SetActive(false);
        else
            panelChat.gameObject.SetActive(true);
    }

    public void UpdateRating(List<UserRating> ratings)
    {
        panelRating.Init(ratings);
    }

    public GameObject CreateUserName(Transform target, string userName, string status)
    {
        GameObject obj = Instantiate(prefabUserName, panelLabel);

        obj.GetComponent<UITargetWorld>().Init(target);
        obj.GetComponent<Text>().text = userName + "\n" + status;
        obj.SetActive(true);
        return obj;
    }

    public void AddFlyText(uint id, Transform target, float val, Color color)
    {
        if (flyTextValues.ContainsKey(id))
        {
            var f = flyTextValues[id];
            if (f != null)
            {
                f.Init(val, color);
                return;
            }
            
            flyTextValues.Remove(id);
        }

        {
            var go = Instantiate(flyText, flyTextRoot);
            var f = go.GetComponent<FlyTextValue>();
            f.Init(val, color);
            go.GetComponent<UITargetWorld>().Init(target);
            go.SetActive(true);
            flyTextValues.Add(id, f);
            go.SetActive(true);
        }
    }

    public UIProgress CreateDurabilityBar(Transform tr)
    {
        GameObject go = Instantiate(durabilityBar, durabilityBarRoot);
        go.GetComponent<UITargetWorld>().Init(tr);
        go.SetActive(true);
        return go.GetComponent<UIProgress>();
    }

    /*
    public void ShowItemInfoItem()
    {
        popupInfo.Init(info);
    }

    public void HideItemInfo(int mode)
    {
        if (popupInfo.Mode == mode)
            popupInfo.gameObject.SetActive(false);
    }
    */

    public void OnMap()
    {
        panelMap.gameObject.SetActive(!panelMap.gameObject.activeSelf);
    }

    public void OnMenu()
    {
        panelMenu.gameObject.SetActive(!panelMenu.gameObject.activeSelf);
    }

    public void ShowPanelYesNo(string text, Action onComplete, Action onCancel)
    {
        panelYesNo.Init(text, onComplete, onCancel);
        panelYesNo.gameObject.SetActive(true);
    }

    public void ShowPanelUserAction(uint userId)
    {
        if (Data.Instance.Clan == null)
            return;

        var u = Data.Instance.Clan.Users.Find(f => f.Id == Data.Instance.UserId);
        if(u == null)
            return;
        if (u.Status != ClanUserStatus.leader)
            return;

        panelUserAction.Init(userId);
        panelUserAction.gameObject.SetActive(true);
    }

    public void ShowResourceInfo(bool isShow, uint objectId)
    {
        /*
        if (isShow)
            popupInfoResource.Init(objectId);
        else
            popupInfoResource.gameObject.SetActive(false);
        */
    }

    public void ShowSelectSector()
    {
        textMainMessage.gameObject.SetActive(true);
        textMainMessage.text = Lang.Get("game", "select_sector");
    }

    public void HideMainMessage()
    {
        textMainMessage.gameObject.SetActive(false);
    }

    public void UpdateCursor()
    {
        switch (cursorSystem.GetCursorMode())
        {
            case CursorMode.normal:
                cursorImage.gameObject.SetActive(false);
                break;
            case CursorMode.build:
                cursorImage.sprite = cursorSprites.Find(f => f.Mode == cursorSystem.GetCursorMode()).Sprite;
                cursorImage.gameObject.SetActive(true);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnCreateUnit()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.createUnit)));
    }

    public void OnSleep()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.sleep)));
    }

    public void ShowRespawn(Vector3 pos)
    {
        respawnSign.GetComponent<UITargetWorld>().Init(pos);
        respawnSign.SetActive(true);
    }

    public void OnSit()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.sit)));
    }

    public void OnShower()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.shower)));
    }

    public void OnOpenDoor()
    {
        //Data.instance.SendMsg(new MsgClient(MsgClintType.pos, new MsgClientPos(MsgClintTypePos.leftClick, pos)));
        Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.exitLocation)));
    }

    public void UpdateUnitAttr()
    {
        var attr = Data.Instance.Unit.Unit.Attr;
        satietyProgress.SetValue(attr[UnitAttrType.satiety] / 100f, attr[UnitAttrType.satiety].ToString("F0") + "/" + "100");
        saturationProgress.SetValue(attr[UnitAttrType.saturation] / 100f, attr[UnitAttrType.saturation].ToString("F0") + "/" + "100");
    }

    public void OnInventory()
    {
        var p = UIPanel<PanelInventory>.Get();
        p.Init(Data.Instance.Unit.Entities.Entities, Data.Instance.Unit.Unit.InventorySize, Data.Instance.Unit.Unit);
        p.Show(!p.gameObject.activeSelf);
    }

    public void UpdateInventory()
    {
        var p = UIPanel<PanelInventory>.Get();
        if(p.gameObject.activeSelf)
            p.Init(Data.Instance.Unit.Entities.Entities, Data.Instance.Unit.Unit.InventorySize, Data.Instance.Unit.Unit);
    }

    public void UpdateContainer(Vector2w pos, List<Entity> items)
    {
        var block = mapSystem.Map.GetBlock(pos);
        var p = UIPanel<global::PanelContainer>.Get();
        p.Init(items, Info.EntityInfo[block.Item1.Id].containerSize, pos);
        p.gameObject.SetActive(true);
    }

    public void OnTasks()
    {
        var p = UIPanel<PanelTasks>.Get();
        p.Init();
        p.ShowChange();
    }

    public void OnCraft()
    {
        OnCraft(Vector2w.Empty);
    }

    public void OnCraft(Vector2w pos)
    {
        var p = UIPanel<PanelCraft>.Get();
        p.Init(pos);
        p.Show(true);
    }

    public void OnSkills()
    {
        var p = UIPanel<PanelSkills>.Get();
        if(!p.gameObject.activeSelf)
            Data.Instance.SendMsg(new MsgClient(MsgClintType.signal, new MsgClientSignal(MsgClintTypeSignal.requestSkills)));
        p.Init();
        p.ShowChange();
    }

    public void ShowPanelSelectItem(Action<ushort> onSelect)
    {
        var p = UIPanel<PanelSelectItem>.Get();
        p.Init(onSelect);
        p.gameObject.SetActive(true);
    }

    public void ShowPanelAddTask()
    {
        panelAddTask.gameObject.SetActive(true);
    }
}
