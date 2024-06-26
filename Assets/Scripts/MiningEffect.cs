using UnityEngine;

public class MiningEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform ps;

    private Transform start;
    private Transform end;

    private float time;

    void Update()
    {
        if (start == null || end == null)
        {
            Destroy(gameObject);
            return;
        }

        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
        ps.transform.position = end.position;

        time -= Time.deltaTime;
        if (time < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Transform start, Transform end, float time)
    {
        this.start = start;
        this.end = end;
        this.time = time;
    }
}
