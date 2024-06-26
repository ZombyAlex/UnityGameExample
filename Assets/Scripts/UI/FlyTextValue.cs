using DG.Tweening;
using TMPro;
using UnityEngine;

public class FlyTextValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform rect;
    
    private float time = 2.0f;

    private float curVaL = 0;
    
    public void Init(float val, Color color)
    {
        curVaL += val;
        string s;
        if(val>0)
            s = "-" + curVaL.ToString("F0");
        else
            s = "miss";

        text.text = s;
        text.color = color;
        time = 2.0f;
        transform.DOScale(Vector3.one * 1.5f, 0.3f).OnComplete((() => { transform.DOScale(Vector3.one, 0.2f); }));
    }

    void Update()
    {
        //rect.anchoredPosition += Vector2.up * Time.deltaTime * 50f;
        time -= Time.deltaTime;
        if (time <= 0)
        {
            transform.DOKill();
            Destroy(gameObject);
        }
    }
}
