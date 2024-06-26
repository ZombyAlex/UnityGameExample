using SWFServer.Data.Entities;
using System;
using UnityEngine;
using UnityTimer;
using VContainer.Unity;

public class TaskTargetSystem :  IInitializable, IDisposable
{
    private GameObject target;
    private Timer timer;

    private MapSystem mapSystem;

    public TaskTargetSystem(MapSystem mapSystem)
    {
        this.mapSystem = mapSystem;
    }

    void OnUpdate()
    {
        if (Data.Instance.LocId == 0 && target == null)
        {
            var task = Data.Instance.tasks.Find(f => f.ExecutorId == Data.Instance.UserId);
            if (task != null)
            {
                var block = mapSystem.Map.GetBlock(task.Pos);
                if (block.Item1 != null)
                    target = PanelMapTarget.instance.AddTarget(mapSystem.ConvPos(block.Item2, 
                        Info.EntityInfo[block.Item1.Id].size, block.Item1.Rotate.Value));
            }
        }

        if (target != null)
        {
            if (Data.Instance.tasks.Find(f => f.ExecutorId == Data.Instance.UserId) == null)
                GameObject.Destroy(target);
        }
    }


    public void Initialize()
    {
        timer = Timer.Register(1f, OnUpdate, null, true);
    }

    public void Dispose()
    {
        Debug.Log("close TaskTargetSystem");
        timer.Cancel();
    }
}
