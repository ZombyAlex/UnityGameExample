using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapTarget
{
    public GameObject go;
    public GameObject goIndicator;
    public Vector2 pos;
}

public class PanelMap : MonoBehaviour
{
    [SerializeField] private GameObject prefabStar;
    [SerializeField] private GameObject prefabPlayer;
    [SerializeField] private GameObject prefabPlanet;
    [SerializeField] private GameObject prefabStation;
    [SerializeField] private GameObject prefabWormhole;
    [SerializeField] private GameObject prefabTarget;
    [SerializeField] private Transform root;
    [SerializeField] private RectTransform rootTransform;

    [SerializeField] private TextMeshProUGUI textCoord;

    private List<GameObject> items = new List<GameObject>();

    private RectTransform player;

    private float factor = 2f;

    void Start()
    {
        prefabStar.SetActive(false);
        prefabPlayer.SetActive(false);
        prefabPlanet.SetActive(false);
        prefabStation.SetActive(false);
        prefabWormhole.SetActive(false);
        prefabTarget.SetActive(false);
    }

    void Update()
    {
        if (player == null)
        {
            player = Instantiate(prefabPlayer, root).GetComponent<RectTransform>();
            player.gameObject.SetActive(true);
        }
        else
        {
            //if (MapSystem.instance.PlayerConnectObject != null)
            {
                //player.anchoredPosition = MapSystem.instance.PlayerConnectObject.transform.position / factor;
            }

        }

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, Input.mousePosition, null, out localPoint);
        Vector2 p = localPoint * factor;
        textCoord.text = "X=" + p.x.ToString("F0") + "  Y=" + p.y.ToString("F0");

        if (rootTransform.rect.Contains(localPoint))
        {
            if (Input.GetMouseButtonDown(0))
            {
                AddTarget();
            }

            if (Input.GetMouseButtonDown(1))
            {
                RemoveTarget();
            }
        }
    }

    private void RemoveTarget()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, Input.mousePosition, null, out localPoint);
        Vector2 p = localPoint * factor;

        for (int i = 0; i < Data.Instance.targets.Count; i++)
        {
            float d = Vector2.Distance(p, Data.Instance.targets[i].pos);
            if (d < 16)
            {
                Data.Instance.Config.RemoveTarget(Data.Instance.LocId, Data.Instance.targets[i].pos);
                Destroy(Data.Instance.targets[i].go);
                Destroy(Data.Instance.targets[i].goIndicator);
                Data.Instance.targets.RemoveAt(i);
                break;
            }
        }
    }

    private void AddTarget()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootTransform, Input.mousePosition, null, out localPoint);
        var go = Instantiate(prefabTarget, root);
        go.GetComponent<RectTransform>().anchoredPosition = localPoint;
        go.SetActive(true);
        var t = new MapTarget() {go = go, pos = localPoint * factor };
        Data.Instance.targets.Add(t);
        Data.Instance.Config.AddTarget(Data.Instance.LocId, t.pos);
        //PanelMapTarget.instance.AddTarget(t);
    }

    public void AddTarget(Vector2 pos)
    {
        var go = Instantiate(prefabTarget, root);
        go.GetComponent<RectTransform>().anchoredPosition = pos / factor;
        go.SetActive(true);
        var t = new MapTarget() { go = go, pos = pos };
        Data.Instance.targets.Add(t);
        //PanelMapTarget.instance.AddTarget(t);
    }

    /*
    public void Init(List<MapInfoItem> list)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            GameObject prefab = null;
            if (list[i].type == SpaceObjectType.star)
                prefab = prefabStar;
            else if (list[i].type == SpaceObjectType.planet)
                prefab = prefabPlanet;
            else if (list[i].type == SpaceObjectType.station)
                prefab = prefabStation;
            else if (list[i].type == SpaceObjectType.wormhole)
                prefab = prefabWormhole;
            Vector2 p = list[i].pos.ToVector2();
            var go = Instantiate(prefab, root);
            go.GetComponent<RectTransform>().anchoredPosition = p / factor;
            go.SetActive(true);
            items.Add(go);
        }
    }
    */
}
