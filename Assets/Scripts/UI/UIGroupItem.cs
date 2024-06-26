using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGroupItem : MonoBehaviour, IItemGroup
{
    [SerializeField] private List<Sprite> backSprites;

    private Image image;
    private UIGroup uiGroup = null;
    private bool isSelect = false;

    private bool isInit = false;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(isInit)
            return;
        isInit = true;
        image = GetComponent<Image>();
        uiGroup = transform.parent.GetComponent<UIGroup>();
        uiGroup?.Register(this);

        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnSelect);
    }

    void Start()
    {
        
    }

    public bool IsSelect()
    {
        return isSelect;
    }

    public void OnSelect()
    {
        Init();
        isSelect = true;
        uiGroup?.Select(this);
    }

    public void Select(bool isSelect)
    {
        Init();
        this.isSelect = isSelect;
        UpdateBack();
    }

    private void UpdateBack()
    {
        image.sprite = isSelect ? backSprites[1] : backSprites[0];
    }

    void OnDestroy()
    {
        uiGroup?.Remove(this);
    }
}
