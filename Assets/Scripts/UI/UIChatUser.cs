using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIChatUser : MonoBehaviour
{
    [SerializeField] private Text text;

    private uint userId;
    private bool isInit = false;

    public uint UserId => userId;

    [Inject] private UIGame uiGame;

    void Update()
    {
        if(isInit)
            return;

        text.text = Data.Instance.GetUserName(userId);
        isInit = !string.IsNullOrEmpty(text.text);
    }

    public void OnSelect()
    {
        uiGame.SelectUserChat(userId);
    }

    public void Init(uint userId)
    {
        this.userId = userId;
        text.text = Data.Instance.GetUserName(userId);
        isInit = !string.IsNullOrEmpty(text.text);
    }
}
