using TMPro;
using UnityEngine;
using VContainer;

public class PanelUserAction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;

    private uint userId;
    [Inject] private UIManager uiManager;
    public void Init(uint userId)
    {
        this.userId = userId;
        textName.text = Data.Instance.GetUserName(userId);
    }

    public void OnInviteClan()
    {
        //Data.instance.SendMsg(new MsgClientInviteClan(userId));
        uiManager.CloseWindow(gameObject);
    }
}
