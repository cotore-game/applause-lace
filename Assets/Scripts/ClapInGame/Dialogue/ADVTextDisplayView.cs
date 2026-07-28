using TMPro;
using UnityEngine;

public sealed class ADVTextDisplayView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI mainText;

    public void SetText(string speakerName, string dialogue)
    {
        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        if (mainText != null)
        {
            mainText.text = dialogue;
        }
    }

    public void Clear()
    {
        SetText(string.Empty, string.Empty);
    }
}
