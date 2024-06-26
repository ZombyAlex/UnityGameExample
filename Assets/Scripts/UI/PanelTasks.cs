using UnityEngine;
using VContainer;

public class PanelTasks : UIPanel<PanelTasks>, IUIPanel
{
    [SerializeField] private PanelItemList<UITask> pool;

    [SerializeField] private GameObject buttonAddTask;

    private int group = 0;

    [Inject] private UIGame uiGame;
    [Inject] private GameContent content;

    public void Init()
    {
        pool.Init();
        pool.ClearItems();

        var list = Data.Instance.tasks;

        foreach (var task in list)
        {
            bool isShow = group == 0 || Data.Instance.LocId == task.LocId;
            if (Data.Instance.LocId == 0)
                isShow = true;

            if (isShow)
            {
                var go = pool.GetItem();
                var item = go.GetComponent<UITask>();
                item.Init(task, content);
                pool.items.Add(item);
                go.transform.SetAsLastSibling();
                go.SetActive(true);
            }
        }

        buttonAddTask.SetActive(Data.Instance.LocationOwner == Data.Instance.UserId);
    }

    public void OnGroup(int n)
    {
        group = n;
        Init();
    }

    public void OnAddTask()
    {
        uiGame.ShowPanelAddTask();
    }
}
