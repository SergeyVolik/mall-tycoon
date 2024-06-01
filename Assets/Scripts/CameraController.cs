using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera m_Camera;
    private Vector3 touchStart;
    private Vector3 moveDelta;
    public float decelerationSpeed;
    public float foolowSpeed;
    private void Awake()
    {      
        m_Camera = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR
        PCInput();
#else
        MobileInput();
#endif
        UpdateCameraPosition();
    }

    private void MobileInput()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = GetWorldPosition(0);
            }

            moveDelta = touchStart - GetWorldPosition(0);
        }
    }

    private void PCInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(0);
        }

        if (Input.GetMouseButton(0))
        {
            moveDelta = touchStart - GetWorldPosition(0);
        }
    }

    private void UpdateCameraPosition()
    {
        var cameraPos = m_Camera.transform.position;
        var targetPos = cameraPos + moveDelta;

        if (cameraPos == targetPos)
            return;

        var t = Mathf.Clamp01(foolowSpeed * Time.deltaTime);
        m_Camera.transform.position = Vector3.Lerp(cameraPos, targetPos, t);
        moveDelta /= decelerationSpeed;
    }

    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = m_Camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}
