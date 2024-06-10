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

            DialogueWindow.GetInstance().onDialogueStarted += GameManager_onDialogueStarted;
            DialogueWindow.GetInstance().onDialogueFinished += GameManager_onDialogueFinished;

        }

        private void GameManager_onDialogueFinished()
        {
            RaycastInput.GetInstance().BlockRaycast = false;
            CameraController.GetInstance().BlockInput = false;
        }

        private void GameManager_onDialogueStarted()
        {
            CameraController.GetInstance().BlockInput = true;
            RaycastInput.GetInstance().BlockRaycast = true;
        }
    }
}