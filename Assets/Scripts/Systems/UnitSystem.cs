using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UnitFactory
{
    private SceneData sceneData;
    private GameConfig config;
    

    public UnitFactory(SceneData sceneData, GameConfig config)
    {
        this.sceneData = sceneData;
        this.config = config;
    }

    public UnitController Create(int type)
    {
        var prefab = type == 0 ? config.Content.prefabUnit : config.Content.prefabCitizen;//TODO
        GameObject instance = Object.Instantiate(prefab, sceneData.unitsParentTransform);
        UnitController controller = instance.GetComponent<UnitController>();
        return controller;
    }
}

public class UnitSystem: ITickable
{
    public Transform PlayerTransform { get; private set; } = null;
    public UnitController PlayerController { get; private set; } = null;

    private Dictionary<uint, UnitController> units = new Dictionary<uint, UnitController>();

    private float timeCheck = 0;

    private MapSystem mapSystem;

    private GameContent content;
    private UIGame uiGame;
    private IObjectResolver container;

    public UnitSystem(MapSystem mapSystem,  GameContent content, UIGame uiGame, IObjectResolver container)
    {
        this.mapSystem = mapSystem;
        //this.unitFactory = unitFactory;
        this.content = content;
        this.uiGame = uiGame;
        uiGame.Init(this);
        this.container = container;
    }

    public void Tick()
    {
        timeCheck -= Time.deltaTime;
        if (timeCheck < 0)
        {
            timeCheck = 0.5f;
            foreach (var unit in units)
            {
                if (!mapSystem.IsMapRect(unit.Value.CellPos))
                {
                    HideUnit(unit.Key);
                    break;
                }
            }
        }
    }

    public void UnitAvatar(UnitAvatar avatar)
    {
        if (units.ContainsKey(avatar.Id))
            units[avatar.Id].Init(avatar);
        else
        {
            var factory = container.Resolve<UnitFactory>();
            UnitController uc = factory.Create(0);//TODO
            container.Inject(uc);

            uc.transform.position = Util.ToVector3(avatar.Pos);

            uc.Init(avatar);
            units.Add(avatar.Id, uc);

            if (avatar.UserId == Data.Instance.UserId)
            {
                PlayerTransform = uc.transform;
                PlayerController = uc;
            }
        }
    }

    public void HideUnit(uint unitId)
    {
        if (units.ContainsKey(unitId))
        {
            GameObject.Destroy(units[unitId].gameObject);
            units.Remove(unitId);
        }
    }

    public void UpdateUnitAttr(UnitAttrType attrType, float val)
    {
        Data.Instance.Unit.Unit.Attr.Set(attrType, val);
    }
}
