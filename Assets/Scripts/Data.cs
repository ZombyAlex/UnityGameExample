using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using UnityEngine;
using VContainer;


[Serializable]
public class ClientSettings
{
    public bool isRemember = false;
    public string login;
    public string pass;
}

public class ClientConfigMapTargetItem
{
    public float x;
    public float y;
}

public class ClientConfigMapTarget
{
    public uint mapId;
    public List<ClientConfigMapTargetItem> targets = new List<ClientConfigMapTargetItem>();
}

public class ClientConfig
{
    public List<ClientConfigMapTarget> MapTarget = new List<ClientConfigMapTarget>();
    public float UIScale = 1f;

    public void AddTarget(uint mapId, Vector2 pos)
    {
        var it = MapTarget.Find(f => f.mapId == mapId);
        if (it == null)
        {
            it = new ClientConfigMapTarget() { mapId = mapId };
            MapTarget.Add(it);
        }

        it.targets.Add(new ClientConfigMapTargetItem() { x = pos.x, y = pos.y});
    }

    public void RemoveTarget(uint mapId, Vector2 pos)
    {
        var it = MapTarget.Find(f => f.mapId == mapId);
        if (it == null)
            return;

        var item = it.targets.Find(f => f.x == pos.x && f.y == pos.y);
        it.targets.Remove(item);
    }
}


public class Data : MonoBehaviour
{
    public static Data Instance;

    [HideInInspector] public bool connected = false;
    [HideInInspector] public VectorLock netMsg = new VectorLock();
    [HideInInspector] public ClientSettings Settings = new ClientSettings();
    [HideInInspector] public ClientConfig Config = new ClientConfig();
    [HideInInspector] public uint UserId;
    [HideInInspector] public uint LocId;
    [HideInInspector] public LocationType LocationType;
    [HideInInspector] public uint LocationOwner;
    [HideInInspector] public int Money;
    [HideInInspector] public double ServerTime;
    [HideInInspector] public Dictionary<uint, string> userNames = new Dictionary<uint, string>();
    [HideInInspector] public Dictionary<uint, bool> requestUserNames = new Dictionary<uint, bool>();
    [HideInInspector] public List<MapTarget> targets = new List<MapTarget>();
    [HideInInspector] public List<GameTask> tasks = new List<GameTask>();


    public Clan Clan { get; set; } = null;
    public Entity Unit { get; set; } = null;

    [Inject] private GameConfig config;

    void Awake()
    {
        Instance = this;
        Lang.Init();
        Info.Init(config.EntitiesInfo.text);
        config.Content.Init();
        LoadSettings();
        LoadConfig();
    }

    public void SendMsg(MsgClient msg)
    {
        netMsg.Add(msg);
    }

    private void LoadSettings()
    {
        string json = PlayerPrefs.GetString("settings", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            Settings = JsonConvert.DeserializeObject<ClientSettings>(json);
        }
    }

    public void SaveSettings()
    {
        string json = JsonConvert.SerializeObject(Settings);
        PlayerPrefs.SetString("settings", json);
    }

    private void LoadConfig()
    {
        string json = PlayerPrefs.GetString("config", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            Config = JsonConvert.DeserializeObject<ClientConfig>(json);
        }
    }

    public void SaveConfig()
    {
        string json = JsonConvert.SerializeObject(Config);
        PlayerPrefs.SetString("config", json);
    }

    public string GetUserName(uint userId)
    {
        if (userNames.ContainsKey(userId))
            return userNames[userId];


        if (!requestUserNames.ContainsKey(userId))
        {
            requestUserNames.Add(userId, true);
            SendMsg(new MsgClient(MsgClintType.id, new MsgClientId(userId, MsgClientTypeId.requestUserName)));
        }

        return string.Empty;
    }

    public void AddUserName(uint userId, string userName)
    {
        if (userNames.ContainsKey(userId))
            userNames[userId] = userName;
        else
            userNames.Add(userId, userName);
    }
}
