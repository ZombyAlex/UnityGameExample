using UnityEngine;
using UnityEngine.UI;

public class UIChatText : MonoBehaviour
{
    public Text text;
    private string copyText;

    
    public void Init(string text, string copyText)
    {
        this.text.text = text;
        this.copyText = copyText;
    }

    public void SetFont(Font font)
    {
        text.font = font;
    }

    public void OnClick()
    {
        GUIUtility.systemCopyBuffer = copyText;
    }
}
