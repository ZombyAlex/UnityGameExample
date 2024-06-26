using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PanelMapTarget : MonoBehaviour
{
    public static PanelMapTarget instance;

    //[SerializeField] private GameObject prefab;

    [Inject] private UIGame uiGame;

    [Inject] private GameConfig config;
    [Inject] private UnitSystem unitSystem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //prefab.SetActive(false);
    }

    public GameObject AddTarget(Vector3 target)
    {
        var it = Instantiate(config.PrefabMapTarget, transform);

        //return null;
        //var go = factory.Create();
        //UIMapTarget it = go.GetComponent<UIMapTarget>();
        it.Init(target, unitSystem);
        it.gameObject.SetActive(true);
        return it.gameObject;
    }

    public void RebuildAllTargets()
    {
        for (int i = 0; i < Data.Instance.targets.Count; i++)
        {
            Destroy(Data.Instance.targets[i].go);
            Destroy(Data.Instance.targets[i].goIndicator);
        }
        Data.Instance.targets.Clear();


        var d = Data.Instance.Config.MapTarget.Find(f => f.mapId == Data.Instance.LocId);
        if (d != null)
        {
            foreach (var it in d.targets)
            {
                uiGame.PanelMap.AddTarget(new Vector2(it.x, it.y));
            }
        }
    }
}
