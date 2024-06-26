using UnityEngine;

public class UITargetWorld : MonoBehaviour
{
    [SerializeField] private float offsetUp;
    private Transform target;

    private Vector3 worldPos;
    private bool isWorldPos = false;


    void LateUpdate()
    {
        if (isWorldPos)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up * offsetUp);
            transform.position = pos;
        }
        else if (target != null)
        {
            Vector3 p = target.position;
            Vector3 pos = Camera.main.WorldToScreenPoint(p + Vector3.up * offsetUp);
            transform.position = pos;
        }
    }

    public void Init(Transform target)
    {
        this.target = target;
    }

    public void Init(Vector3 pos)
    {
        worldPos = pos;
        isWorldPos = true;
    }
}
