using SWFServer.Data.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PanelLogin : UIPanel<PanelLogin>, IUIPanel
{
    [SerializeField] private TMP_InputField fieldLogin;
    [SerializeField] private TMP_InputField fieldPass;
    [SerializeField] private Toggle toggleRemember;

    private UIMenu uiMenu;

    [Inject]
    public void Construct(UIMenu uiMenu)
    {
        this.uiMenu = uiMenu;
    }

    void Start()
    {
        toggleRemember.isOn = Data.Instance.Settings.isRemember;
        if (Data.Instance.Settings.isRemember)
        {
            fieldLogin.text = Data.Instance.Settings.login;
            fieldPass.text = Data.Instance.Settings.pass;
        }
    }

    public void OnClick()
    {
        if (string.IsNullOrEmpty(fieldLogin.text) || string.IsNullOrEmpty(fieldPass.text))
            return;

        Data.Instance.Settings.isRemember = toggleRemember.isOn;
        if (Data.Instance.Settings.isRemember)
        {
            Data.Instance.Settings.login = fieldLogin.text;
            Data.Instance.Settings.pass = fieldPass.text;
        }
        else
        {
            Data.Instance.Settings.login = string.Empty;
            Data.Instance.Settings.pass = string.Empty;
        }

        Data.Instance.SaveSettings();

        Data.Instance.SendMsg(new MsgClient(MsgClintType.login,  new MsgClientLogin(fieldLogin.text, fieldPass.text)));
        gameObject.SetActive(false);

        uiMenu.ShowPanelConnecting(true);
        GameManager.Instance.StartLogin();
    }
}
