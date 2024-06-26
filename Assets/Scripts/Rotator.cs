using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float speed = 90;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}
