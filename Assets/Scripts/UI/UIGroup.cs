using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup : MonoBehaviour
{
    private List<IItemGroup> items = new List<IItemGroup>();
    
    void Start()
    {
        
    }

    public void Register(IItemGroup item)
    {
        items.Add(item);
    }

    public void Select(IItemGroup item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Select(items[i] == item);
        }
    }

    public void Remove(IItemGroup item)
    {
        items.Remove(item);
    }
}
