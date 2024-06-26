using SWFServer.Data.Entities;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "GameContent", menuName = "Configs/GameContent")]
public class GameContent : ScriptableObject
{
    public List<Sprite> items = new List<Sprite>();

    public List<GameObject> blocks = new List<GameObject>();

    public Sprite iconDefault;
    public List<Color> colorProgress;
    public GameObject itemPrefab;
    public List<GameObject> blockPrefabs;
    public List<Color> SkinColors;

    public List<Sprite> iconMining;

    public GameObject prefabUnit;
    public GameObject prefabCitizen;


    private Dictionary<string, Sprite> itemsDic = new Dictionary<string, Sprite>();
    private Dictionary<string, GameObject> blockDic = new Dictionary<string, GameObject>();
    private Dictionary<ushort, GameObject> blockDicId = new Dictionary<ushort, GameObject>();

#if UNITY_EDITOR
    [Button]
    void UpdateBlocks()
    {
        blocks.Clear();
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/World/SWF.fbx");

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject b = obj.transform.GetChild(i).gameObject;
            blocks.Add(b);
        }
        EditorUtility.SetDirty(this);
    }

    [Button]
    void UpdateItems()
    {
        items.Clear();

        string[] giuds = AssetDatabase.FindAssets("t:texture", new[] { "Assets/Art/Items" });

        for (int i = 0; i < giuds.Length; i++)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(giuds[i])) as TextureImporter;
            if (textureImporter.textureType != TextureImporterType.Sprite || textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.isReadable = true;

                AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(giuds[i]));
            }

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(giuds[i]));
            items.Add(s);
        }

        EditorUtility.SetDirty(this);
    }
#endif

    public void Init()
    {
        Debug.Log("Init content");

        foreach (var block in blocks)
        {
            blockDic.Add(block.name, block);
        }

        foreach (var item in Info.EntityInfo.items)
        {
            var it = blockPrefabs.Find(f => f.name == item.name);
            if (it == null)
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
