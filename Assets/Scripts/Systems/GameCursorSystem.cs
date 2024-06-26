using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using SWFServer.Data;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public enum CursorMode
{
    normal,
    build
}

public struct EventChangeCursorMode
{
    public CursorMode Mode;
}

public struct EventClickMap
{
    public Vector2w Pos;
}

public class GameCursorSystem: ITickable
{

    public CursorMode CursorMode { get; private set; } = CursorMode.normal;


    private SceneData sceneData;
    private ModeSystem modeSystem;
    private MapSystem mapSystem;
    private MapPosSystem mapPosSystem;
    private GameConfig config;
    private UIManager uiManager;


    [Inject]
    public GameCursorSystem(SceneData sceneData, ModeSystem modeSystem, MapSystem mapSystem, MapPosSystem mapPosSystem, GameConfig config,
         UIManager uiManager)
    {
        this.sceneData = sceneData;
        this.modeSystem = modeSystem;
        this.mapSystem = mapSystem;
        this.mapPosSystem = mapPosSystem;
        this.config = config;
        this.uiManager = uiManager;
    }

    public void Tick()
    {
        if (modeSystem.IsGame)
        {
            if (!sceneData.GameCursor.activeSelf)
                sceneData.GameCursor.SetActive(true);
            var p = mapSystem.ConvPos(mapPosSystem.MapPos)
                    - new Vector3(config.CellSize / 2, -0.03f, config.CellSize / 2);

            sceneData.GameCursor.transform.position = Vector3.Lerp(sceneData.GameCursor.transform.position, p, Time.deltaTime * 20f);
        }
        else
        {
            if (sceneData.GameCursor.activeSelf)
                sceneData.GameCursor.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CursorMode != CursorMode.normal)
            {
                SetCursorMode(CursorMode.normal);
                Messenger.Send(new EventUpdateCursor());
            }
        }
    }

    public void SetCursorMode(CursorMode mode)
    {
        if (CursorMode == mode)
            CursorMode = CursorMode.normal;
        else
            CursorMode = mode;

        Messenger.Send(new EventChangeCursorMode() { Mode = mode });


        switch (CursorMode)
        {
            case CursorMode.normal:
                break;
            case CursorMode.build:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    public void ClickMap(Vector2w pos)
    {
        if (uiManager.IsOverInterface())
            return;

        if (!mapSystem.IsMapPos(pos))
            return;

        Messenger.Send(new EventClickMap() { Pos = pos });

        if (CursorMode == CursorMode.normal)
        {
            Data.Instance.SendMsg(new MsgClient(MsgClintType.pos, new MsgClientPos(MsgClintTypePos.leftClick, pos)));

            var block = mapSystem.Map.GetBlock(pos);
            if (block.Item1 != null)
            {
                if (Info.EntityInfo[block.Item1.Id].workbenchType != WorkbenchType.not)
                {
                    Messenger.Send(new EventOnCraft(){pos = pos});
                }

                if (Info.EntityInfo[block.Item1.Id].tradeSize > 0 && block.Item1.Entities != null && block.Item1.Entities.Entities.Count > 0)
                {
                    var p = UIPanel<PanelBuy>.Get();
                    p.Init(block.Item1.Entities.Entities, pos);
                    p.Show(true);
                }
            }
        }
        else if (CursorMode == CursorMode.build)
        {
            //SetCursorMode(CursorMode.normal);
        }
    }

    public void ClickMapRect(WRect rect)
    {
        if (CursorMode == CursorMode.normal)
            return;

        if (uiManager.IsOverInterface())
            return;

        for (int x = 0; x < rect.w; x++)
        {
            for (int y = 0; y < rect.h; y++)
            {
                Vector2w p = new Vector2w(rect.x + x, rect.y + y);
                ClickMap(p);
            }
        }
    }

    public CursorMode GetCursorMode()
    {
        return CursorMode;
    }
}
