using System;
using UnityEngine;
using VContainer;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;

    private Transform target;
    private Action onComplete;
    private float damage;
    private uint targetId;

    [Inject] private UIGame uiGame;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime*speed);
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            uiGame.AddFlyText(targetId, target, damage, Color.red);
            onComplete();
            Destroy(gameObject);
        }
    }

    public void Init(Transform target, uint targetId, float damage, Action onComplete)
    {
        this.target = target;
        this.damage = damage;
        this.targetId = targetId;
        this.onComplete = onComplete;
    }
}
