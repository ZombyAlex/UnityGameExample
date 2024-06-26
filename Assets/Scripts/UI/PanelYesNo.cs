using System;
using TMPro;
using UnityEngine;
using VContainer;

public class PanelYesNo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textInfo;

    private Action onComplete;
    private Action onCancel;

    [Inject] private UIManager uiManager;
    public void Init(string text, Action onComplete, Action onCancel)
    {
        textInfo.text = text;
        this.onComplete = onComplete;
        this.onCancel = onCancel;
    }

    public void OnYes()
    {
        onComplete();
        uiManager.CloseWindow(gameObject);
    }

    public void OnNo()
    {
        onCancel();
        uiManager.CloseWindow(gameObject);
    }
}
