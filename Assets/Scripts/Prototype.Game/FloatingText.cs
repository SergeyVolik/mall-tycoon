using UnityEngine;

namespace Prototype
{
    public class FloatingText : MonoBehaviour
    {
        public TMPro.TextMeshPro textField;
        private Transform m_Camera;
        private Animator m_Animator;

        private void Awake()
        {
            m_Camera = Camera.main.transform;
            m_Animator = GetComponent<Animator>();
        }

        public void Show(string text)
        {
            textField.text = text;
            m_Animator.Play("FloatingTextAnimation");
        }
        
        
        private void Update()
        {
            transform.forward = m_Camera.forward;
        }
    }
}
