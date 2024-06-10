using System;
using UnityEngine;

namespace Prototype
{
    [System.Serializable]
    public class DialogueItem
    {
        public DialogueCharacterSO dialogueCharacter;
        [TextArea(3, 6)]
        public string text;
    }

    [CreateAssetMenu]
    public class DialogueSequenceSO : ScriptableObject
    {
        public DialogueItem[] dialogueItems;

        public void StartDialogue()
        {
            DialogueWindow.GetInstance().StartDialogue(this);
        }

        public event Action onFinished = delegate { };
        public event Action onStarted = delegate { };
        public event Action<DialogueItem> onNextItem = delegate { };

        public void OnFinish()
        {
            onFinished.Invoke();
        }

        public void OnStart()
        {
            onFinished.Invoke();
        }

        public void OnNextDialogueItem(DialogueItem item)
        {
            onNextItem.Invoke(item);
        }
    }
}
