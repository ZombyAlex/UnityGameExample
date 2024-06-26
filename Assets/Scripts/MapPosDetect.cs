using SWFServer.Data;
using UnityEngine;
using VContainer;

public class MapPosDetect : MonoBehaviour
{
    public static MapPosDetect instance;

    [SerializeField] private Camera gameCamera;

    private Vector2w startMapPos;

    [Inject] private MapPosSystem mapPosSystem;
    [Inject] private MapSystem mapSystem;
    [Inject] private GameCursorSystem cursorSystem;
    [Inject] private UnitSystem unitSystem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
        if (!mapSystem.IsMap)
            return;

        if (mapSystem.IsMapPos(mapPosSystem.MapPos))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //start rect
                startMapPos = mapPosSystem.MapPos;
            }

            WRect rect = new WRect(Mathf.Min(startMapPos.x, mapPosSystem.MapPos.x), Mathf.Min(startMapPos.y, mapPosSystem.MapPos.y),
                Mathf.Abs(startMapPos.x - mapPosSystem.MapPos.x) + 1, Mathf.Abs(startMapPos.y - mapPosSystem.MapPos.y) + 1);

            if (Input.GetMouseButtonUp(0))
            {
                if (rect.w > 1 || rect.h > 1)
                    cursorSystem.ClickMapRect(rect);
                else
                    cursorSystem.ClickMap(mapPosSystem.MapPos);
            }

            if (Input.GetMouseButtonUp(1) && mapPosSystem.MapPos.GetR(unitSystem.PlayerController.CellPos) < 2)
            {
                var block = mapSystem.Map.GetBlock(mapPosSystem.MapPos);
                if (block.Item1 != null)
                {
                    var p = UIPanel<PanelAction>.Get();
                    p.Init(block.Item1, block.Item2);
                    p.Show(true);
                }
            }
        }
    }
}
