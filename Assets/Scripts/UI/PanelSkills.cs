using SWFServer.Data.Entities;
using SWFServer.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PanelSkills : UIPanel<PanelSkills>, IUIPanel
{
    [SerializeField] private PanelItemList<UIImageTexts> pool;

    [Inject] private GameContent content;

    public void Init()
    {
        pool.Init();
        pool.ClearItems();

        var listM = Data.Instance.Unit.Unit.Skills.Mining;

        foreach (var it in listM)
        {
            var go = pool.GetItem();
            var item = go.GetComponent<UIImageTexts>();
            item.Init(content.iconMining[(int)it.Key], new List<string>()
            {
                Lang.Get("game", "m_"+it.Key),
                "Lvl: " + Data.Instance.Unit.Unit.Skills.Level(it.Key),
                "Exp: " + it.Value.ToString("F1")
            });
            go.SetActive(true);
            pool.items.Add(item);
        }

        var list = Data.Instance.Unit.Unit.Skills.Craft;

        foreach (var it in list)
        {
            var go = pool.GetItem();
            var item = go.GetComponent<UIImageTexts>();
            item.Init(content[Info.EntityInfo[it.Key].name], new List<string>()
            {
                Lang.Get("items", Info.EntityInfo[it.Key].name),
                "Lvl: " + Data.Instance.Unit.Unit.Skills.Level(it.Key),
                "Exp: " + it.Value.ToString("F1")
            });
            go.SetActive(true);
            pool.items.Add(item);
        }
    }
}
