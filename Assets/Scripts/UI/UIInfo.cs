using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private List<UIInfoItem> items = new List<UIInfoItem>();
    
    void Start()
    {
        prefab.SetActive(false);
    }

    public void AddInfo(string tag)
    {
        string s = Lang.Get("notification", tag);
        GameObject obj = Instantiate(prefab, transform);
        obj.GetComponent<TextMeshProUGUI>().text = s;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -130);
        var it = obj.GetComponent<UIInfoItem>();
        it.Init(new Vector2(0, items.Count * -20f), this);
        items.Add(it);
        obj.SetActive(true);
    }

    public void RemoveInfo(UIInfoItem info)
    {
        items.Remove(info);

        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdatePos(new Vector2(0, i * -20f));
        }
    }
}
