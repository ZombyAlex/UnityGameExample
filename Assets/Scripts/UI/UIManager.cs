using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIManager : MonoBehaviour
{
    //public static UIManager instance;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject game;

    [SerializeField] private List<RectTransform> uiRects;
    [SerializeField] private Camera worldCamera;

    private bool isClickWindow = false;

    private Canvas canvas;
    private RectTransform canvasRectTransform;

    public GameObject Game => game;


    private UIMenu uiMenu;

    [Inject]
    public void Construct(UIMenu uiMenu)
    {
        this.uiMenu = uiMenu;
    }

    void Awake()
    {
        //instance = this;

        canvas = GetComponent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        menu.GetComponent<UIMenu>().Awake();

        var list = GetComponentsInChildren<IUIPanel>(true);

        foreach (var p in list)
        {
            p.PreInit();
        }
    }

    void Start()
    {
        SetGameMode(false);

        SetUIScale(Data.Instance.Config.UIScale);
    }

    void Update()
    {
        
    }

    public void ShowWrongVersionClient()
    {
        uiMenu.ShowWrongVersionClient();
    }

    public void SetGameMode(bool isGame)
    {
        menu.SetActive(!isGame);
        game.SetActive(isGame);
    }

    public void OnClickWindow()
    {
        isClickWindow = true;
    }
    public void OnClickWindowReset()
    {
        isClickWindow = false;
    }

    public bool IsOverInterface()
    {
        
        if (isClickWindow)
        {
            isClickWindow = false;
            return true;
        }
        

        foreach (RectTransform rect in uiRects)
        {
            if (!rect.gameObject.activeSelf || !rect.gameObject.activeInHierarchy)
                continue;
            if (GetWorldRect(rect, rect.lossyScale).Contains(Input.mousePosition))
                return true;
        }

        return false;
    }

    public bool IsMouseRect(RectTransform rect)
    {
        if (!rect.gameObject.activeSelf || !rect.gameObject.activeInHierarchy)
            return false;
        if (GetWorldRect(rect, rect.lossyScale).Contains(Input.mousePosition))
            return true;
        return false;
    }

    public Rect GetWorldRect(RectTransform rt, Vector2 scale)
    {
        // Convert the rectangle to world corners and grab the top left
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 topLeft = corners[0];

        // Rescale the size appropriately based on the current Canvas scale
        Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

        return new Rect(topLeft, scaledSize);
    }


    public void CloseWindow(GameObject window)
    {
        OnClickWindow();

        CloseAllHelpers(window);

        window.SetActive(false);
    }

    public void CloseAllHelpers(GameObject window)
    {
        
        var list = window.GetComponentsInChildren<UIItemHelper>();
        for (int i = 0; i < list.Length; i++)
        {
            list[i].OnPointerExit(null);
        }
        /*
        var list2 = window.GetComponentsInChildren<UIItem>();
        for (int i = 0; i < list2.Length; i++)
        {
            list2[i].OnPointerExit(null);
        }
        */
    }

    public Vector2 GetMousePosInCanvas()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, null, out pos);
        return pos;
    }

    public void SetUIScale(float val)
    {
        GetComponent<CanvasScaler>().scaleFactor = val;
    }
}
