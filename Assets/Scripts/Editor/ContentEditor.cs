using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Content))]
public class ContentEditor : Editor
{
    private Vector2Int[] offset = new[] { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) };

    public override void OnInspectorGUI()
    {
        Content c = (Content)target;

        if (GUILayout.Button("Update blocks"))
        {
            c.blocks.Clear();
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/World/SWF.fbx");

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject b = obj.transform.GetChild(i).gameObject;
                c.blocks.Add(b);
            }
            EditorUtility.SetDirty(c);
        }


        if (GUILayout.Button("Update items"))
        {
            c.items.Clear();

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
                c.items.Add(s);
            }

            EditorUtility.SetDirty(c);
        }
        

        DrawDefaultInspector();
    }
}
