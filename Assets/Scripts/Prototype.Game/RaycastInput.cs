using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Prototype
{
    public interface IActivateableFromRaycast
    {
        public void ActivateFromRaycast();
    }

    public class RaycastInput : Singleton<RaycastInput>
    {
        public bool blockRaycast = false;
        private void Awake()
        {
            m_Camera = Camera.main;
            m_Hits = new RaycastHit[10];
        }

        private void Update()
        {
            if (blockRaycast)
            {
                return;
            }

#if UNITY_EDITOR
            PCInput();
#else
            MobileInput();
#endif
        }

        private Camera m_Camera;
        private Vector3 startDragCamPos;
        private RaycastHit[] m_Hits;
        private void MobileInput()
        {
            if (Input.touchCount == 0)
                return;

            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startDragCamPos = m_Camera.transform.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if ((startDragCamPos - m_Camera.transform.position).magnitude > 0.5f)
                {
                    return;
                }

                var ray = m_Camera.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit))
                {
                    return;
                }

                if (hit.collider.TryGetComponent<IActivateableFromRaycast>(out var toActivate))
                {
                    toActivate.ActivateFromRaycast();
                }
            }
        }

        private void PCInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startDragCamPos = m_Camera.transform.position;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if ((startDragCamPos - m_Camera.transform.position).magnitude > 0.5f)
                {
                    //Debug.Log("magnitude is to low");
                    return;
                }

                var ray = m_Camera.ScreenPointToRay(Input.mousePosition);

                //Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.5f);
                int len = Physics.RaycastNonAlloc(ray, m_Hits);

                if (len == 0)
                {
                    //Debug.Log("no raycast target");
                    return;
                }

                for (int i = 0; i < len; i++)
                {
                    var item = m_Hits[i];
                    if (item.collider.TryGetComponent<IActivateableFromRaycast>(out var toActivate))
                    {
                        toActivate.ActivateFromRaycast();
                        break;
                    }
                }
            }
        }
    }
}
