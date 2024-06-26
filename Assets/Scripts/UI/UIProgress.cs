using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIProgress : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textVal;

    [Inject] private GameContent content;

    public void SetValue(float val, bool isColor = false)
    {
        image.fillAmount = val;
        if (isColor)
        {
            Color c;
            if (val < 0.2f)
                c = content.colorProgress[2];
            else if (val < 0.5f)
                c = content.colorProgress[1];
            else
                c = content.colorProgress[0];
            image.color = c;
        }
    }

    public void SetValue(float val, string text)
    {
        image.fillAmount = val;
        textVal.text = text;
    }
}
