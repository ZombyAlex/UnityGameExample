using System;
using SWFServer.Data;
using SWFServer.Data.Entities;
using TMPro;
using UnityEngine;
using VContainer;

public class PopupInfo : UIPanel<PopupInfo>, IUIPanel
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rectTransform;

    /*
    private UnitInfoItem unitInfo;
    private ModuleInfoItem moduleInfo;
    private ItemInfoItem itemInfo;
    */

    //public int Mode { get; private set; } = 0;

    //private float time = 0;

    //private Entity entity;

    [Inject] private UIManager uiManager;

    private void UpdateSize()
    {
        Vector2 size = rectTransform.sizeDelta;
        size.y = text.preferredHeight + 10;

        size.x = text.preferredWidth + 10;
        /*
        if (text.preferredWidth > 214)
            size.x = text.preferredWidth + 10;
        else
            size.x = 224;
        */
        rectTransform.sizeDelta = size;
    }

    void Update()
    {
        Vector2 size = rectTransform.sizeDelta;

        float x, y;
        if (Input.mousePosition.x > Screen.width / 2)
            x = -size.x / 2 - 16;
        else
            x = size.x / 2 + 16;

        if (Input.mousePosition.y > Screen.height / 2)
            y = -size.y / 2 - 16;
        else
            y = size.y / 2 + 16;
        rectTransform.anchoredPosition = uiManager.GetMousePosInCanvas() + new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    private void ResetInfo()
    {
        //entity = null;
    }

    public void Init(Entity entity)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        //if(this.entity == entity)
        //    return;
        ResetInfo();
        //this.entity = entity;

        var t = CreateInfo(entity);


        //t += "\n" + Lang.Get("game", "cargo") + ": " + info.cargo;
        //t += "\n" + Lang.Get("game", "storage") + ": " + info.storage;

        /*
        for (int i = 0; i < info.moduleLevel.Count; i++)
        {
            string im = Lang.Get("modules", info.moduleLevel[i].type.ToString());
            t += "\n" + im + ": " + info.moduleLevel[i].level;
        }

        if (info.requirements != null)
        {
            t += "\n" + "<color=green>" + Lang.Get("game", "requirements") + "</color>";
            t += "\n" + Lang.Get("skills", info.requirements.type.ToString()) + ": " + info.requirements.level;
        }
        */

        text.text = t;
        UpdateSize();
    }

    private string CreateInfo(Entity entity)
    {
        ResetInfo();

        var info = Info.EntityInfo[entity.Id];

        string t = String.Empty;

        t += "<color=#ed7614>" + Lang.Get("items", info.name) + "</color>";
        if (info.durability > 0)
            t += "\n" + Lang.Get("game", "durability") + ": " + entity.Durability.Value + "/" + info.durability.ToString("F0");

        if (info.containerSize > 0 && entity.Entities.Entities.Count > 0)
        {
            t += "\n" + "<color=green>" + Lang.Get("game", "container") + "</color>";
            foreach (var en in entity.Entities.Entities)
            {
                t += "\n " + Lang.Get("items", Info.EntityInfo[en.Id].name) + ": " + en.Count.Value;
            }
        }

        if (info.containerSize == 0 && entity.Entities!= null && entity.Entities.Entities.Count > 0)
        {
            t += "\n" + "<color=green>" + Lang.Get("game", "items") + "</color>";
            foreach (var en in entity.Entities.Entities)
            {
                t += "\n " + Lang.Get("items", Info.EntityInfo[en.Id].name) + ": " + en.Count.Value;
            }
        }

        if (entity.UserId != null && entity.UserId.Value != 0)
        {
            var n = Data.Instance.GetUserName(entity.UserId.Value);
            t += "\n" + Lang.Get("game", "owner") + ": " + n;
        }

        if (entity.ExitLocId != null) 
            t += "\n" + Lang.Get("game", "exit_location") + ": " + entity.ExitLocId.Value;

        if (entity.Cost != null)
            t += "\n" + Lang.Get("game", "cost") + ": " + entity.Cost.Value;

        return t;
    }


    /*
    public void Init(ModuleInfoItem info)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (moduleInfo == info)
            return;
        ResetInfo();
        moduleInfo = info;
        Mode = 1;

        string t = String.Empty;

        t += "<color=#ed7614>" + Lang.Get("items", info.name) + "</color>";

        //t += "\n" + Lang.Get("game", "type") + ": " + info.type;
        t += "\n" + Lang.Get("game", "durability") + ": " + info.durability.ToString("F0");
        t += "\n" + Lang.Get("game", "levelClass") + ": " + Lang.Get("modules", info.levelClass.ToString());
        t += "\n" + Lang.Get("game", "level") + ": " + info.level;

        if (info.type == ModuleType.engine)
        {
            t += "\n" + Lang.Get("game", "speed") + ": " + info.speed;
            t += "\n" + Lang.Get("game", "speedRotate") + ": " + info.speedRotate;
            t += "\n" + Lang.Get("game", "acceleration") + ": " + info.acceleration;
            t += "\n" + Lang.Get("game", "energyMax") + ": " + info.energyMax;
            t += "\n" + Lang.Get("game", "energyUse") + ": " + info.energyUse;
            t += "\n" + Lang.Get("game", "durabilityUse") + ": " + info.durabilityUse;

            t += "\n" + Lang.Get("game", "bootsUse") + ": " + info.bootsUse;
            t += "\n" + Lang.Get("game", "boostSpeed") + ": " + info.boostSpeed;
            t += "\n" + Lang.Get("game", "boostEnergy") + ": " + info.boostEnergy;
        }
        else if (info.type == ModuleType.reactor)
        {
            t += "\n" + Lang.Get("game", "energyMax") + ": " + info.energyMax;
            t += "\n" + Lang.Get("game", "energyUse") + ": " + info.energyUse;
            t += "\n" + Lang.Get("game", "durabilityUse") + ": " + info.durabilityUse;
        }
        else if (info.type == ModuleType.weapon)
        {
            t += "\n" + Lang.Get("game", "range") + ": " + info.range;
            t += "\n" + Lang.Get("game", "timeReload") + ": " + info.timeReload;
            t += "\n" + Lang.Get("game", "damage") + ": " + info.damage;

            t += "\n" + Lang.Get("game", "energyMax") + ": " + info.energyMax;
            t += "\n" + Lang.Get("game", "energyUse") + ": " + info.energyUse;
            t += "\n" + Lang.Get("game", "durabilityUse") + ": " + info.durabilityUse;
            if (info.mining > 0)
                t += "\n" + Lang.Get("game", "mining") + ": " + info.mining;

            if(info.isMiningGas)
                t += "\n" + Lang.Get("game", "gas_mining");
        }
        else if (info.type == ModuleType.charge)
        {
            t += "\n" + Lang.Get("game", "damage") + ": " + info.damage;
            t += "\n" + Lang.Get("game", "speed") + ": " + info.speed;
            t += "\n" + Lang.Get("game", "speedRotate") + ": " + info.speedRotate;
            t += "\n" + Lang.Get("game", "acceleration") + ": " + info.acceleration;
            if (info.life > 0)
                t += "\n" + Lang.Get("game", "life") + ": " + info.life;
        }
        else if (info.type == ModuleType.protection)
        {
            t += "\n" + Lang.Get("game", "energyMax") + ": " + info.energyMax;
            t += "\n" + Lang.Get("game", "durabilityUse") + ": " + info.durabilityUse;
        }
        else if (info.type == ModuleType.powerGenerator)
        {
            t += "\n" + Lang.Get("game", "durabilityUse") + ": " + info.durabilityUse;
            t += "\n" + Lang.Get("game", "energyUse") + ": " + info.energyUse;
        }

        if (info.requirements != null)
        {
            t += "\n" + "<color=green>" + Lang.Get("game", "requirements") + "</color>";
            t += "\n" + Lang.Get("skills", info.requirements.type.ToString()) + ": " + info.requirements.level;
        }


        text.text = t;
        UpdateSize();
    }

    public void Init(ItemInfoItem info)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        if (itemInfo == info)
            return;
        ResetInfo();
        itemInfo = info;
        Mode = 2;


        string t = String.Empty;
        t += "<color=#ed7614>" + Lang.Get("items", info.name) + "</color>";
        t += "\n" + Lang.Get("game", "cost") + ": " + info.cost;
        if (info.energy > 0)
            t += "\n" + Lang.Get("game", "energy") + ": " + info.energy;
        
        text.text = t;
        UpdateSize();
    }

    public void Init(uint objectId)
    {
        if (!Data.instance.ResourceInfo.ContainsKey(objectId))
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        var it = Data.instance.ResourceInfo[objectId];
        string t = String.Empty;

        for (int i = 0; i < it.Count; i++)
        {
            var item = it[i];
            t += Lang.Get("items", Info.ItemInfo[item.Id].name) + ": " + item.Count+ "\n";
        }

        text.text = t;
        UpdateSize();

    }
    */
    public void Init(MapCell cell, Entity block)
    {
        string t = String.Empty;

        if (block != null)
            t += CreateInfo(block);
        if (cell.Floor != null)
        {
            if (!string.IsNullOrEmpty(t)) t += "\n-----\n";
            t += CreateInfo(cell.Floor);
        }
        /*
        if (cell.Ground != null && cell.Floor == null)
        {
            if (!string.IsNullOrEmpty(t)) t += "\n-----\n";
            t += CreateInfo(cell.Ground);
        }
        */
        if (!string.IsNullOrEmpty(t))
        {
            text.text = t;
            UpdateSize();
            Update();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
