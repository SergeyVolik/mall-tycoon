using UnityEngine;

namespace Prototype
{
    public class GameManager : MonoBehaviour
    {
        [Range(30, 60)]
        public int targetFPS = 60;
        private void Awake()
        {
            Application.targetFrameRate = targetFPS;
        }
    }
}