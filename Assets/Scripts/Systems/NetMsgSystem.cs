using SWFServer.Data.Net;
using SWFServer.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class NetMsgSystem: ITickable
{

    private List<MsgServer> msgs = new List<MsgServer>();

    private UIMenu uiMenu;
    private UIGame uiGame;
    private UIInfo uiInfo;
    private MapSystem mapSystem;
    private UnitSystem unitSystem;
    private ModeSystem modeSystem;

    public NetMsgSystem(UIMenu uiMenu, UIGame uiGame, UIInfo uiInfo, MapSystem mapSystem, UnitSystem unitSystem, ModeSystem modeSystem)
    {
        this.uiMenu = uiMenu;
        this.uiGame = uiGame;
        this.uiInfo = uiInfo;
        this.mapSystem = mapSystem;
        this.unitSystem = unitSystem;
        this.modeSystem = modeSystem;
    }

    public void Tick()
    {
        while (msgs.Count > 0)
        {
            ProcMsg(msgs[0]);
            msgs.RemoveAt(0);
        }
    }

    public void AddMsg(MsgServer msg)
    {
        msgs.Add(msg);
    }

    private void ProcMsg(MsgServer msg)
    {
        //Debug.Log("msg= " + msg.Type);
        switch (msg.Type)
        {
            case MsgServerType.connect:
                {
                    MsgServerConnect m = (MsgServerConnect)msg.Data;

                    GameManager.net.Reconnect(m.port);
                    uiMenu.OnLogin();
                }
                break;
            case MsgServerType.user:
                {
                    MsgServerUser m = (MsgServerUser)msg.Data;
                    Data.Instance.UserId = m.userId;
                    Data.Instance.LocationType = m.locationType;
                    Data.Instance.LocationOwner = m.locationOwner;

                    if (modeSystem.IsGame)
                    {
                        if (Data.Instance.LocId != m.mapId)
                        {
                            mapSystem.ClearAll();
                            //Data.instance.ResourceInfo.Clear();
                        }
                    }
                    Data.Instance.LocId = m.mapId;

                    PanelMapTarget.instance.RebuildAllTargets();

                    uiGame.UpdateUID();
                    if (!modeSystem.IsGame)
                        modeSystem.SetGameMode(true);

                    uiMenu.ShowPanelConnecting(false);
                }
                break;
            case MsgServerType.map:
                {
                    MsgServerMap m = (MsgServerMap)msg.Data;
                    //Debug.Log("map = " + msg.Data.LengthBytes + " p=" + m.objects.Count);

                    mapSystem.UpdateMap(m.sectors, m.sectorPos);
                }
                break;


            case MsgServerType.time:
                {
                    MsgServerTime m = (MsgServerTime)msg.Data;
                    Data.Instance.ServerTime = m.time;
                    Debug.Log("time = " + m.time);
                }
                break;
            case MsgServerType.info:
                {
                    MsgServerInfo m = (MsgServerInfo)msg.Data;
                    uiInfo.AddInfo(m.info);
                }
                break;
            case MsgServerType.userName:
                {
                    MsgServerUserName m = (MsgServerUserName)msg.Data;
                    Data.Instance.AddUserName(m.reqUserId, m.userName);
                }
                break;



            case MsgServerType.userList:
                {
                    MsgServerUserList m = (MsgServerUserList)msg.Data;
                    uiGame.UpdateUserList(m.users);
                }
                break;
            case MsgServerType.chat:
                {
                    MsgServerChat m = (MsgServerChat)msg.Data;
                    uiGame.ChatMsg(m);
                }
                break;

            case MsgServerType.rating:
                {
                    MsgServerRating m = (MsgServerRating)msg.Data;
                    uiGame.UpdateRating(m.ratings);
                }
                break;

            case MsgServerType.signal:
                {
                    MsgServerSignal m = (MsgServerSignal)msg.Data;
                    switch (m.signal)
                    {
                        case MsgServerTypeSignal.exitBattle:

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                break;
            case MsgServerType.mapCell:
                {
                    MsgServerMapCell m = (MsgServerMapCell)msg.Data;
                    mapSystem.UpdateMapCell(m.cell, m.pos);
                }
                break;
            case MsgServerType.unitAvatar:
                {
                    MsgServerUnitAvatar m = (MsgServerUnitAvatar)msg.Data;
                    unitSystem.UnitAvatar(m.avatar);
                }
                break;
            case MsgServerType.mapCellLayer:
                {
                    MsgServerMapCellLayer m = (MsgServerMapCellLayer)msg.Data;
                    mapSystem.UpdateMapCellLayer(m.cellLayer, m.pos, m.layer);
                }
                break;
            case MsgServerType.unit:
                {
                    MsgServerUnit m = (MsgServerUnit)msg.Data;
                    Data.Instance.Unit = m.unit;
                    uiGame.UpdateUnitAttr();
                    uiGame.UpdateInventory();
                    UIPanel<PanelSkills>.Get().Init();
                }
                break;
            case MsgServerType.hideUnit:
                {
                    MsgServerHideUnit m = (MsgServerHideUnit)msg.Data;
                    unitSystem.HideUnit(m.unitId);
                }
                break;
            case MsgServerType.unitAttr:
                {
                    MsgServerUnitAttr m = (MsgServerUnitAttr)msg.Data;
                    unitSystem.UpdateUnitAttr(m.attrType, m.val);
                    uiGame.UpdateUnitAttr();
                }
                break;
            case MsgServerType.money:
                {
                    MsgServerMoney m = (MsgServerMoney)msg.Data;
                    Data.Instance.Money = m.money;
                    uiGame.UpdateMoney();
                }
                break;
            case MsgServerType.inventory:
                {
                    MsgServerInventory m = (MsgServerInventory)msg.Data;
                    if (m.pos == Vector2w.Empty)
                    {
                        Data.Instance.Unit.Entities.Entities = m.items;
                        uiGame.UpdateInventory();
                    }
                    else
                    {
                        uiGame.UpdateContainer(m.pos, m.items);
                    }
                }
                break;
            case MsgServerType.unitAction:
                {
                    MsgServerUnitAction m = (MsgServerUnitAction)msg.Data;
                    UIPanel<PanelUnitAction>.Get().Init(m.time);
                    UIPanel<PanelUnitAction>.Get().Show(true);
                }
                break;
            case MsgServerType.tasks:
                {
                    MsgServerTasks m = (MsgServerTasks)msg.Data;
                    Data.Instance.tasks = m.tasks;
                    UIPanel<PanelTasks>.Get().Init();
                }
                break;
            case MsgServerType.entity:
                {
                    MsgServerEntity m = (MsgServerEntity)msg.Data;
                    if (m.pos == -1)
                        Data.Instance.Unit.Unit.HandItem = m.entity;
                    else
                        Data.Instance.Unit.Entities.Entities[m.pos] = m.entity;

                    uiGame.UpdateInventory();
                }
                break;
            default:
                {
                    Debug.LogError("not msg type = " + msg.Type);
                }
                break;
        }
    }

    
}
