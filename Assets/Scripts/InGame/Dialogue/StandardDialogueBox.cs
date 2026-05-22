using UnityEngine;
using TMPro;

namespace GameDialogue
{
    /// <summary>
    /// 標準的な対話ボックス実装
    /// </summary>
    public class StandardDialogueBox : MonoBehaviour, IDialogueBox
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform balloonTransform;
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Transform characterImageTransform;

        public CanvasGroup GetCanvasGroup() => canvasGroup;
        public RectTransform GetBalloonTransform() => balloonTransform;
        public TMP_Text GetSpeakerNameText() => speakerNameText;
        public TMP_Text GetMessageText() => messageText;
        public Transform GetCharacterImageTransform() => characterImageTransform;
    }
}
