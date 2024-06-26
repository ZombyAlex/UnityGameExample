using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class UIChatRClick : MonoBehaviour, IPointerDownHandler
{
    [Inject] private UIGame uiGame;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            uiGame.ShowPanelUserAction(GetComponent<UIChatUser>().UserId);
        }
    }
}
