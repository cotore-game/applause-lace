using UnityEngine;
using TMPro;

namespace GameDialogue
{
    /// <summary>
    /// 対話ボックスのインターフェース
    /// </summary>
    public interface IDialogueBox
    {
        CanvasGroup GetCanvasGroup();
        RectTransform GetBalloonTransform();
        TMP_Text GetSpeakerNameText();
        TMP_Text GetMessageText();
        Transform GetCharacterImageTransform();
    }
}
