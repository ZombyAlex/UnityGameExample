using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BuildSystem : IInitializable, ITickable, IDisposable
{
    private GameObject buildObject = null;
    private ushort buildId;
    private byte buildRotate = 0;
    private int buildInventoryPos = 0;

    [Inject] private MapPosSystem mapPosSystem;
    [Inject] private MapSystem mapSystem;
    [Inject] private GameContent content;
    [Inject] private GameCursorSystem cursorSystem;


    public void Initialize()
    {
        Messenger.AddListener<EventChangeCursorMode>(OnChangeCursorMode);
        Messenger.AddListener<EventClickMap>(OnClickMap);
        Messenger.AddListener<EventStartBuild>(OnStartBuild);
    }

    public void Dispose()
    {
        Messenger.RemoveListener<EventChangeCursorMode>(OnChangeCursorMode);
        Messenger.RemoveListener<EventClickMap>(OnClickMap);
        Messenger.RemoveListener<EventStartBuild>(OnStartBuild);
    }

    public void Tick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (buildObject != null)
            {
                buildRotate++;
                if (buildRotate > 3)
                    buildRotate = 0;
            }
        }

        if (buildObject != null)
        {
            var size = Info.EntityInfo[buildId].size;
            if (buildRotate == 1 || buildRotate == 3)
                size.Swap();
            Vector3 p = new Vector3((size.x - 1) / 2f, 0, (size.y - 1) / 2f);
            var info = Info.EntityInfo[buildId];

            Vector3 up = Vector3.zero;
            if (info.layer == EntityMapLayer.floor)
                up = Vector3.up * 0.1f;

            buildObject.transform.position = mapSystem.ConvPos(mapPosSystem.MapPos) + p + up;
            buildObject.transform.rotation = Quaternion.Euler(0, 90 * buildRotate, 0);
        }
    }

    private void OnStartBuild(EventStartBuild obj)
    {
        ShowBuildObj(obj.Item, obj.InventPos);
    }

    private void OnClickMap(EventClickMap obj)
    {
        if (cursorSystem.CursorMode == CursorMode.build)
        {
            Data.Instance.SendMsg(new MsgClient(MsgClintType.build, new MsgClientBuild(buildInventoryPos, obj.Pos, buildRotate)));
        }
    }

    private void OnChangeCursorMode(EventChangeCursorMode obj)
    {
        switch (obj.Mode)
        {
            case CursorMode.normal:
                if (buildObject != null)
                    GameObject.Destroy(buildObject);
                break;
            case CursorMode.build:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void ShowBuildObj(Entity item, int inventoryPos)
    {
        var prefab = content.Block(item.Id);
        buildRotate = 0;
        buildId = item.Id;
        buildInventoryPos = inventoryPos;
        var size = Info.EntityInfo[item.Id].size;
        if (buildRotate == 1 || buildRotate == 3)
            size.Swap();
        Vector3 p = new Vector3((size.x - 1) / 2f, 0, (size.y - 1) / 2f);
        var info = Info.EntityInfo[buildId];

        buildObject = GameObject.Instantiate(prefab, mapSystem.ConvPos(mapPosSystem.MapPos, info.size, buildRotate) + p,
            Quaternion.Euler(0, 90 * buildRotate, 0));
    }
}
