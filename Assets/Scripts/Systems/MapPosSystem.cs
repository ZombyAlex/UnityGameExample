using SWFServer.Data;
using UnityEngine;
using VContainer.Unity;

public class MapPosSystem: ITickable
{
    private SceneData sceneData;
    private GameConfig gameConfig;
    private MapSystem mapSystem;
    private UIManager uiManager;

    public Vector2w MapPos = Vector2w.Zero;
    private Vector2w LastMapPos = Vector2w.Zero;
    private Plane plane = new Plane(Vector3.zero, Vector3.forward, Vector3.right);
    private float time = 0;

    private bool isShow = false;

    public MapPosSystem(SceneData sceneData, GameConfig gameConfig, MapSystem mapSystem, UIManager uiManager)
    {
        this.sceneData = sceneData;
        this.gameConfig = gameConfig;
        this.mapSystem = mapSystem;
        this.uiManager = uiManager;
    }

    public void Tick()
    {
        var p = GetMousePos();
        MapPos = new Vector2w((int)(p.x / gameConfig.CellSize), (int)(p.y / gameConfig.CellSize));

        if (LastMapPos != MapPos)
        {
            LastMapPos = MapPos;
            time = 0;
            if (isShow) HideInfo();
        }

        if (!uiManager.IsOverInterface())
        {
            time += Time.deltaTime;
            if (time > 0.3f)
            {
                if (mapSystem.Map.IsMap(MapPos))
                {
                    var cell = mapSystem.Map[MapPos];
                    if (cell != null)
                    {
                        var panel = UIPanel<PopupInfo>.Get();
                        var block = mapSystem.Map.GetBlock(MapPos);
                        if (block.Item1 != null || cell.Floor != null)
                        {
                            panel.Init(cell, block.Item1);
                            panel.Show(true);
                            isShow = true;
                        }
                    }

                }
            }
        }
        else
        {
            if (isShow) HideInfo();
        }

    }

    private void HideInfo()
    {
        isShow = false;
        var panel = UIPanel<PopupInfo>.Get();
        panel.Show(false);
    }

    private Vector2 GetMousePos()
    {
        Ray ray = sceneData.MainCamera.ScreenPointToRay(Input.mousePosition);
        float d;
        plane.Raycast(ray, out d);
        var p = ray.GetPoint(d);
        return new Vector2(p.x, p.z);
    }
}
