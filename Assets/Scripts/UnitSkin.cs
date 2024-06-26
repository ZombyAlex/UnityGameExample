using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkin : MonoBehaviour
{
    [SerializeField] private GameObject hat;
    [SerializeField] private MeshRenderer hatRenderer;
    [SerializeField] private Material materialFree;
    [SerializeField] private Material materialColor;

    [SerializeField] private List<SkinnedMeshRenderer> body;
    [SerializeField] private List<SkinnedMeshRenderer> legs;
    
    /*
    public void UpdateHat(bool isShow, int color)
    {
        hat.SetActive(isShow);
        if (isShow)
        {
            hatRenderer.material.SetColor("_Color", Content.instance.SkinColors[color]);
        }
    }

    public void UpdateBody(bool isShow, int color)
    {
        for (int i = 0; i < body.Count; i++)
        {
            body[i].material = isShow ? materialColor : materialFree;
            if(isShow)
                body[i].material.SetColor("_Color", Content.instance.SkinColors[color]);
        }
    }

    public void UpdateLegs(bool isShow, int color)
    {
        for (int i = 0; i < legs.Count; i++)
        {
            legs[i].material = isShow ? materialColor : materialFree;
            if (isShow)
                legs[i].material.SetColor("_Color", Content.instance.SkinColors[color]);
        }
    }

    public void Init(List<ushort> equip, List<int> colors)
    {
        UpdateHat(equip[2] != 0, colors[0]);
        UpdateBody(equip[3] != 0, colors[1]);
        UpdateLegs(equip[4] != 0, colors[2]);
    }
    */
}
