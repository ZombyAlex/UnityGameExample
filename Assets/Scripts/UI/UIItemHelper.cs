using SWFServer.Data.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemHelper : MonoBehaviour  , IPointerEnterHandler, IPointerExitHandler
{
    //private UnitInfoItem unitInfoItem;
    //private ModuleInfoItem moduleInfoItem;
    //private ItemInfoItem itemInfoItem;

    private RectTransform rectTransform;
    private Entity entity;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void Init(Entity entity)
    {
        this.entity = entity;
    }

    void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPoint);

        if (rectTransform.rect.Contains(localPoint)) 
            Show();
    }

    public void Show()
    {
       
        if (entity != null)
            UIPanel<PopupInfo>.Get().Init(entity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("show " + GetInstanceID() + "  " + eventData.pointerEnter.gameObject.name);
        Show();
        /*
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.pointerCurrentRaycast.screenPosition))
        {
            Debug.Log("show " + GetInstanceID() + "  " + eventData.pointerEnter.gameObject.name);
            Show();
        }
        */
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if (!eventData.fullyExited) return;
        
        //if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.pointerCurrentRaycast.screenPosition))
        {
            //Debug.Log("hide " + GetInstanceID() + "  " + eventData.pointerEnter.gameObject.name);
            Hide();
        }
    }

    private void Hide()
    {
        UIPanel<PopupInfo>.Get().Show(false);
    }
}
