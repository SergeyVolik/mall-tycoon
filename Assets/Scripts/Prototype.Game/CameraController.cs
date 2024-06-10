using System;
using UnityEngine;

namespace Prototype
{
    public class CameraController : Singleton<CameraController>
    {
        private Camera m_Camera;
        private Vector3 m_TouchStart;
        private Vector3 m_MoveDelta;
        public Transform cameraTarget;
        public Vector3 cameraOffset;
        [Header("Movement")]
        public float decelerationSpeed;

        [Header("Zoom")]
        public float zoomSpeedMult = 1f;
        public float minZoom = 5;
        public float maxZoom = 7;

        public Collider cameraBounds;

        public float cameraFollowSpeed = 10f;
        public float cameraForseTargetFollowSpeed = 10f;

        private void Awake()
        {
            m_Camera = Camera.main;
            m_Camera.orthographicSize = maxZoom;
            m_Camera.transform.position = cameraTarget.position + cameraOffset;
        }

        void Update()
        {
            CameraMovement();
            CameraZoom();
        }

        float prevZoomDistance;
        private bool m_BlockMovement;

        float GetTouchZoomDelta()
        {
            if (Input.touchCount < 2)
            {
                return 0;
            }

            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);

            var distance = Vector2.Distance(touch1.position, touch2.position);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                prevZoomDistance = distance;
            }

            var delta = prevZoomDistance - distance;
            prevZoomDistance = distance;

            return delta / Screen.width;
        }

        void CameraZoom()
        {
#if UNITY_EDITOR
            var increment = Input.GetAxis("Mouse ScrollWheel");
#else
        var increment = GetTouchZoomDelta();
#endif
            increment *= zoomSpeedMult;
            m_Camera.orthographicSize = Mathf.Clamp(m_Camera.orthographicSize + increment, minZoom, maxZoom);
        }

        private void CameraMovement()
        {

            if (!BlockInput)
            {
#if UNITY_EDITOR
                PCInput();
#else
        MobileInput();
#endif
            }
            UpdateCameraPosition();
        }

        bool m_MoveInputStarted = false;
        private Transform m_ForceTarget;

        public bool BlockInput { get; internal set; }

        private void MobileInput()
        {
            if (Input.touchCount > 1)
            {
                m_BlockMovement = true;
                return;
            }
            else if (m_BlockMovement && Input.touchCount == 0)
            {
                m_BlockMovement = false;
            }

            if (m_BlockMovement)
                return;

            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    m_TouchStart = GetWorldPosition(0);
                    m_MoveInputStarted = true;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    m_MoveInputStarted = false;
                }

                if (m_MoveInputStarted)
                {
                    m_MoveDelta = m_TouchStart - GetWorldPosition(0);
                }
            }
        }

        private void PCInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_MoveInputStarted = true;
                m_TouchStart = GetWorldPosition(0);
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_MoveInputStarted = false;
            }

            if (m_MoveInputStarted)
            {
                m_MoveDelta = m_TouchStart - GetWorldPosition(0);
            }
        }

        private void UpdateCameraPosition()
        {
            var cameraTargetPosition = cameraTarget.position;

            float speed = cameraFollowSpeed;

            if (m_ForceTarget)
            {
                speed = cameraForseTargetFollowSpeed;
                cameraTargetPosition = m_ForceTarget.position;
                cameraTargetPosition.y = cameraTarget.position.y;
            }
        
            var targetPos = cameraTargetPosition + m_MoveDelta;

            var bounds = cameraBounds.bounds;
            targetPos.x = Mathf.Clamp(targetPos.x, bounds.min.x, bounds.max.x);
            targetPos.z = Mathf.Clamp(targetPos.z, bounds.min.z, bounds.max.z);

            targetPos = Vector3.Lerp(cameraTarget.position, targetPos, Mathf.Clamp01(Time.deltaTime * speed));
            cameraTarget.position = targetPos;
            m_Camera.transform.position = targetPos + cameraOffset;
            m_MoveDelta /= decelerationSpeed;
        }

        private Vector3 GetWorldPosition(float z)
        {
            Ray mousePos = m_Camera.ScreenPointToRay(Input.mousePosition);
            Plane ground = new Plane(Vector3.up, new Vector3(0, 0, z));
            float distance;
            ground.Raycast(mousePos, out distance);
            return mousePos.GetPoint(distance);
        }

        internal void ForceTarget(Transform traget)
        {
            m_ForceTarget = traget;
        }
    }
}