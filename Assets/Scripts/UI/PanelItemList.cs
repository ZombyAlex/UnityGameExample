using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PanelItemList<T> where T: MonoBehaviour
{
    [SerializeField, Required] private GameObject prefabItem;
    [SerializeField, Required] private Transform rootItems;

    [HideInInspector] public List<T> items = new List<T>();
    protected List<T> pool = new List<T>();

    public void Init()
    {
        prefabItem.SetActive(false);
    }

    public void ClearItems()
    {
        items.ForEach(f =>
        {
            f.gameObject.SetActive(false);
            pool.Add(f);
        });

        items.Clear();
    }

    public GameObject GetItem()
    {
        if (pool.Count > 0)
        {
            var go = pool[0];
            pool.RemoveAt(0);
            return go.gameObject;
        }

        return GameObject.Instantiate(prefabItem, rootItems);
    }
}
