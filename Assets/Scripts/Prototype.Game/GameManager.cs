using UnityEngine;

namespace Prototype
{
    public class GameManager : MonoBehaviour
    {
        [Range(30, 60)]
        public int targetFPS = 60;

        public DialogueSequenceSO firstDialogue;

        private void Awake()
        {
            Application.targetFrameRate = targetFPS;

            firstDialogue.StartDialogue();

            DialogueWindow.GetInstance().onDialogueStarted += GameManager_onDialogueStarted;
            DialogueWindow.GetInstance().onDialogueFinished += GameManager_onDialogueFinished;

        }

        private void GameManager_onDialogueFinished()
        {
            RaycastInput.GetInstance().BlockRaycast = false;
            CameraController.GetInstance().enabled = true;
        }

        private void GameManager_onDialogueStarted()
        {
            CameraController.GetInstance().enabled = false;
            RaycastInput.GetInstance().BlockRaycast = true;
        }
    }
}