using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class UIDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] public Transform drag;
    private bool dragging;
    private Vector3 lastPos;

    private RectTransform rect;
    private RectTransform area;

    [Inject] private UIManager uiManager;
    public void Start()
    {
        rect = drag.GetComponent<RectTransform>();
        area = uiManager.Game.GetComponent<RectTransform>();
        
        string nx = "win_x_" + drag.name;
        string ny = "win_y_" + drag.name;

        Vector3 p = drag.position;

        if (PlayerPrefs.HasKey(nx)) 
            p.x = PlayerPrefs.GetFloat(nx);
        if (PlayerPrefs.HasKey(ny)) 
            p.y = PlayerPrefs.GetFloat(ny);

        drag.position = p;
    }

    public void Update()
    {
        if (dragging)
        {
            Vector3 offs = lastPos - Input.mousePosition;
            offs.z = 0;
            lastPos = Input.mousePosition;
            drag.position -= offs;
        }
        else
        {
            float s = 500;
            if (rect.anchoredPosition.x - rect.sizeDelta.x / 2 < area.rect.xMin)
                drag.position += Vector3.right * Time.deltaTime * s;

            if (rect.anchoredPosition.y - rect.sizeDelta.y / 2 < area.rect.yMin)
                drag.position += Vector3.up * Time.deltaTime * s;


            if (rect.anchoredPosition.x + rect.sizeDelta.x / 2 > area.rect.xMax)
                drag.position -= Vector3.right * Time.deltaTime * s;

            if (rect.anchoredPosition.y + rect.sizeDelta.y / 2 > area.rect.yMax)
                drag.position -= Vector3.up * Time.deltaTime * s;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
        lastPos = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        Save();
    }

    private void Save()
    {
        string nx = "win_x_" + drag.name;
        string ny = "win_y_" + drag.name;

        PlayerPrefs.SetFloat(nx, drag.position.x);
        PlayerPrefs.SetFloat(ny, drag.position.y);
    }
}
