using System;
using System.Collections.Generic;
using UnityEngine;


public interface IUIPanel
{
    public void PreInit();
}

public class UIPanel<T> : MonoBehaviour where T : MonoBehaviour, IUIPanel
{
    private static Dictionary<Type, UIPanel<T>> panels = new Dictionary<Type, UIPanel<T>>();

    protected static void Add(UIPanel<T> panel)
    {
        if (!panels.ContainsKey(typeof(T)))
        {
            panels.Add(typeof(T), panel);
        }
    }

    public static T Get()
    {
        if (panels.TryGetValue(typeof(T), out UIPanel<T> panel))
        {
            return panel as T;
        }
        return default;
    }

    public void PreInit()
    {
        Add(this);
    }

    public void Show(bool isShow)
    {
        if (gameObject.activeSelf != isShow)
            gameObject.SetActive(isShow);
    }

    public void ShowChange()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
