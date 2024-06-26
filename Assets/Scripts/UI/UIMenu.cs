using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    //public static UIMenu instance;

    //[SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelConnecting;
    [SerializeField] private GameObject panelErrorVersion;
    
    public void Awake()
    {
        //instance = this;

        int lang = PlayerPrefs.GetInt("cur_lang", 0);
        Lang.SetLang(lang);
    }

    public void OnStart(bool isLocal)
    {
        GameManager.Instance.Connect(isLocal);
    }

    public void OnLogin()
    {
        UIPanel<PanelLogin>.Get().Show(true);
        //panelLogin.SetActive(true);
    }

    public void OnLang(int lang)
    {
        Lang.SetLang(lang);
        PlayerPrefs.SetInt("cur_lang", lang);
    }

    public void ShowPanelConnecting(bool isShow)
    {
        panelConnecting.SetActive(isShow);
    }

    public void ShowWrongVersionClient()
    {
        panelErrorVersion.SetActive(true);
    }
}
