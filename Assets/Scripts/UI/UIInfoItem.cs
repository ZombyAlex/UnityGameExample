using TMPro;
using UnityEngine;


public class UIInfoItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private Vector2 target;
    private RectTransform rt;
    private float life = 5f;

    private UIInfo uiInfo;
    
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, target, Time.deltaTime*10f);

        life -= Time.deltaTime;
        if (life < 0)
        {
            uiInfo.RemoveInfo(this);
            Destroy(gameObject);
        }
    }

    public void Init(Vector2 target, UIInfo uiInfo)
    {
        this.uiInfo = uiInfo;
        this.target = target;
    }

    public void UpdatePos(Vector2 pos)
    {
        target = pos;
    }
}
