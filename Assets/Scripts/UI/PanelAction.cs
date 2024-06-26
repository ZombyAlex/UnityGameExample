using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using UnityEngine;
using VContainer;

public class PanelAction : UIPanel<PanelAction>, IUIPanel
{
    [SerializeField] private GameObject buttonGet;

    private Entity entity;
    private Vector2w pos;

    private RectTransform rectTransform;

    [Inject] private UIManager uiManager;

    public void Init(Entity entity, Vector2w pos)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchoredPosition = uiManager.GetMousePosInCanvas();

        this.entity = entity;
        this.pos = pos;

        var info = Info.EntityInfo[entity.Id];

        buttonGet.SetActive(info.isTake);

        if (!buttonGet.activeSelf)
            gameObject.SetActive(false);
    }

    public void OnGet()
    {
        Data.Instance.SendMsg(new MsgClient(MsgClintType.pos, new MsgClientPos(MsgClintTypePos.takeBlock, pos)));
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!uiManager.IsMouseRect(rectTransform))
                gameObject.SetActive(false);
        }
    }
}
