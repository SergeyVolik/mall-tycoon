using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Prototype
{
    public class DialogueWindow : Singleton<DialogueWindow>
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI text;
        public Button nextDialogueButton;

        public event Action onDialogueFinished = delegate { };
        public event Action onDialogueStarted = delegate { };

        private int m_DialogueItemIndex = 0;
        private DialogueSequenceSO m_CurrentDialogue;
        public float timeBetweenCharacters = 0.1f;
        private void Awake()
        {
            nextDialogueButton.onClick.AddListener(() =>
            {
                if (!m_AnimationIsPlaying)
                {
                    NextDialogueItem();
                }
                else {
                    SkipAnimation();
                }
            });

            FinishDialogue();
        }

        public void StartDialogue(DialogueSequenceSO newDialogue)
        {
            m_CurrentDialogue = newDialogue;
            gameObject.SetActive(true);
            m_DialogueItemIndex = 0;

            var dialogueItem = m_CurrentDialogue.dialogueItems[m_DialogueItemIndex];
            SetupDialogueItem(dialogueItem);
            m_CurrentDialogue.OnStart();
            m_CurrentDialogue.OnNextDialogueItem(dialogueItem);
            onDialogueStarted.Invoke();
        }

        private void SetupDialogueItem(DialogueItem dialogueItem)
        {
            title.text = dialogueItem.dialogueCharacter.characterName;
            StartAnimation();
        }

        public void NextDialogueItem()
        {
            m_DialogueItemIndex++;

            if (m_DialogueItemIndex >= m_CurrentDialogue.dialogueItems.Length)
            {
                FinishDialogue();
                return;
            }

            var dialogueItem = m_CurrentDialogue.dialogueItems[m_DialogueItemIndex];

            m_CurrentDialogue.OnNextDialogueItem(dialogueItem);
            SetupDialogueItem(dialogueItem);
          
        }

        private DialogueItem GetCurrentDialogueItem() => m_CurrentDialogue.dialogueItems[m_DialogueItemIndex];
        public void FinishDialogue()
        {
            gameObject.SetActive(false);
            if (m_CurrentDialogue)
            {
                m_CurrentDialogue.OnFinish();
                m_CurrentDialogue = null;
            }

            onDialogueFinished.Invoke();
        }

        bool m_AnimationIsPlaying = false;
        private Coroutine m_Coroutine;

        private void SkipAnimation()
        {
            m_AnimationIsPlaying = false;
            text.text = GetCurrentDialogueItem().text;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
        }

        private void StartAnimation()
        {
            text.text = "";
            m_Coroutine = StartCoroutine(WriteTextAnimation());
        }
        private IEnumerator WriteTextAnimation()
        {
            m_AnimationIsPlaying = true;
            text.text = "";
            foreach (var item in GetCurrentDialogueItem().text.ToCharArray())
            {
                text.text += item;
                yield return new WaitForSeconds(timeBetweenCharacters);
            }

            m_AnimationIsPlaying = false;
        }
    }
}
