using System.Collections.Generic;
using SWFServer.Data;
using UnityEngine;

public class PanelRating : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform rootItems;

    private List<GameObject> items = new List<GameObject>();
    
    void Start()
    {
        prefab.SetActive(false);
    }

    public void Init(List<UserRating> ratings)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }
        items.Clear();

        for (int i = 0; i < ratings.Count; i++)
        {
            GameObject obj = Instantiate(prefab, rootItems);

            obj.GetComponent<UIRating>().Init(ratings[i], i + 1);
            obj.SetActive(true);
            items.Add(obj);
        }

    }
}
