using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Messenger
{
    private static readonly Dictionary<Type, List<Delegate>> Handlers = new Dictionary<Type, List<Delegate>>();

    public static void AddListener<T>(Action<T> callback)
    {
        List<Delegate> dic = null;
        if (!Handlers.TryGetValue(typeof(T), out dic))
        {
            dic = new List<Delegate>();
            Handlers[typeof(T)] = dic;
        }

        if (!dic.Contains(callback))
        {
            //Debug.Log("AddListener " + callback);
            dic.Add(callback);
        }
    }

    public static void RemoveListener<T>(Action<T> callback)
    {
        List<Delegate> list = null;
        if (Handlers.TryGetValue(typeof(T), out list))
        {
            //Debug.Log("RemoveListener " + callback);
            list.Remove(callback);
        }
    }

    public static void Send<T>(T msg)
    {
        List<Delegate> list = null;
        if (!Handlers.TryGetValue(typeof(T), out list))
            return;
        //Debug.Log("send " + msg + " [" + list.Count + "]");
        foreach (var action in list.Cast<Action<T>>())
        {
            
            action(msg);
        }
    }

}
