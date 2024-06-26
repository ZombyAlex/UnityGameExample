using System;
using System.Collections.Generic;
using SWFServer.Data.Entities;
using UnityEngine;


public class Content : MonoBehaviour
{
    //public static Content instance;

    public List<Sprite> items = new List<Sprite>();
    
    public List<GameObject> blocks = new List<GameObject>();

    private Dictionary<string, Sprite> itemsDic = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> blockDic = new Dictionary<string, GameObject>();
    private Dictionary<ushort, GameObject> blockDicId = new Dictionary<ushort, GameObject>();


    public Sprite iconDefault;

    public List<Color> colorProgress;
    public GameObject itemPrefab;
    public List<GameObject> blockPrefabs;

    public List<Color> SkinColors;


    void Awake()
    {
        //instance = this;

        foreach (var block in blocks)
        {
            blockDic.Add(block.name, block);
        }

        foreach (var item in Info.EntityInfo.items)
        {
            var it = blockPrefabs.Find(f => f.name == item.name);
            if(it == null)
                it = blocks.Find(f => f.name == item.name);
            if (it != null)
                blockDicId.Add(item.id, it);
        }

        foreach (var sprite in items)
        {
            itemsDic.Add(sprite.name, sprite);
        }
    }
    public Sprite this[string id]
    {
        get
        {
            if (itemsDic.ContainsKey(id))
                return itemsDic[id];
            return iconDefault;
        }
    }

    public GameObject Block(string id)
    {
        if (blockDic.ContainsKey(id))
            return blockDic[id];
        return itemPrefab;
    }

    public GameObject Block(ushort id)
    {
        if (blockDicId.ContainsKey(id))
            return blockDicId[id];
        return itemPrefab;
    }


    public Sprite GetSprite(string id)
    {
        if (itemsDic.ContainsKey(id))
            return itemsDic[id];
        return null;
    }
}
