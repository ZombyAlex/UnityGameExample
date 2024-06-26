using UnityEngine;
using UnityEngine.UI;

public class PanelUnitAction : UIPanel<PanelUnitAction>, IUIPanel
{
    [SerializeField] private Image progress;

    private float time;

    void Update()
    {
        float s = 1f / time;
        progress.fillAmount += Time.deltaTime * s;
        if (progress.fillAmount >= 1)
            gameObject.SetActive(false);
    }

    public void Init(float time)
    {
        this.time = time;
        progress.fillAmount = 0;
    }
}
