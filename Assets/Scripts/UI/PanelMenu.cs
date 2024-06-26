using UnityEngine;
using VContainer;

public class PanelMenu : MonoBehaviour
{
    
    [Inject] private UIManager uiManager;

    public void OnExit()
    {
        Application.Quit();
    }

    public void SetUIScale(float val)
    {
        Data.Instance.Config.UIScale = val;
        uiManager.SetUIScale(val);
    }
}
