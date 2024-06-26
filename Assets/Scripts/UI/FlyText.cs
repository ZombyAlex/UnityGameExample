using TMPro;
using UnityEngine;

public class FlyText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rect;
    
    private float time = 0.6f;
    
    public void Init(string str, Color color)
    {
        text.text = str;
        text.color = color;
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * Time.deltaTime * 50f;
        time -= Time.deltaTime;
        if (time <= 0)
        {
            Destroy(gameObject);
        }
    }
}
