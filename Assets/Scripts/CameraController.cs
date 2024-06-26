using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float startDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float zoomRate;
    [SerializeField] private float rotationRateY;
    [SerializeField] private float rotationRateX;
    [SerializeField] private float minRotationX;
    [SerializeField] private float maxRotationX;
    [SerializeField] private Transform compass;
    [SerializeField] private Transform target;


    private float curDistance;
    private float targetDistance;

    private bool isMiddleButtonDown = false;
    private Vector3 lastPosMiddleButton = Vector3.zero;
    private Quaternion targetRotation;

    private float curRotationX;

    private float initialDistance;
    private float initialScale;

    [Inject] private UIManager uiManager;
    [Inject] private UnitSystem unitSystem;

    void Start()
    {
        curDistance = startDistance;
        targetDistance = curDistance;
        targetRotation = transform.rotation;
        curRotationX = transform.rotation.eulerAngles.x;
    }

    void LateUpdate()
    {
        var tr = unitSystem.PlayerTransform;
        if (tr == null)
            tr = target;
        if (tr != null)
        {
            if (!uiManager.IsOverInterface())
            {
                float axisWheel = Input.GetAxis("Mouse ScrollWheel");
                targetDistance = targetDistance - axisWheel * zoomRate;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }

            if (Application.isMobilePlatform)
                UpdateZoom();

            curDistance = Mathf.Lerp(curDistance, targetDistance, Time.deltaTime * 5);

            UpdateRotation();
            var pos = tr.position - transform.forward * curDistance;
            transform.position = pos;
        }
    }

    private void UpdateZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = targetDistance;
            }

            float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

            float scaleFactor = (currentDistance / initialDistance);

            targetDistance = initialScale / scaleFactor;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
    }

    private void UpdateRotation()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isMiddleButtonDown = true;
            lastPosMiddleButton = Input.mousePosition;

        }

        if (Input.GetMouseButtonUp(2))
        {
            isMiddleButtonDown = false;

        }

        if (isMiddleButtonDown)
        {
            Vector3 delta = lastPosMiddleButton - Input.mousePosition;
            lastPosMiddleButton = Input.mousePosition;

            if (Mathf.Abs(delta.x) > 0)
            {
                targetRotation =  Quaternion.Euler(0, -delta.x * rotationRateY, 0)* targetRotation;
            }

            if (Mathf.Abs(delta.y) > 0)
            {
                float oldVal = curRotationX;
                curRotationX += delta.y * rotationRateX;
                curRotationX = Mathf.Clamp(curRotationX, minRotationX, maxRotationX);
                float d = curRotationX - oldVal;

                targetRotation = targetRotation * Quaternion.Euler(d, 0, 0);
            }


            transform.rotation = targetRotation;
        }

        if (compass != null)
            compass.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.y + 90);
    }
}
