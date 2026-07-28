using UnityEngine;
using UnityEngine.UI;

public sealed class CharacterView : MonoBehaviour
{
    [SerializeField] private Image characterImage;

    public void SetPortrait(Sprite portrait)
    {
        if (characterImage == null)
        {
            return;
        }

        characterImage.sprite = portrait;
        characterImage.enabled = portrait != null;
    }

    public void ClearPortrait()
    {
        SetPortrait(null);
    }
}
