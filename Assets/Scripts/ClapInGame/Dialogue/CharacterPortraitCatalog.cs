using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterPortraitCatalog",
    menuName = "Game/Dialogue/CharacterPortraitCatalog")]
public sealed class CharacterPortraitCatalog : ScriptableObject
{
    [Serializable]
    private sealed class ExpressionSprite
    {
        [SerializeField] private CharacterExpression expression;
        [SerializeField] private Sprite sprite;

        public CharacterExpression Expression => expression;
        public Sprite Sprite => sprite;
    }

    [Serializable]
    private sealed class CharacterDefinition
    {
        [SerializeField] private DialogueSpeaker speaker;
        [SerializeField] private string displayName;
        [SerializeField] private ExpressionSprite[] expressions;

        public DialogueSpeaker Speaker => speaker;
        public string DisplayName => displayName;
        public ExpressionSprite[] Expressions => expressions;
    }

    [SerializeField] private CharacterDefinition[] characters;

    public string GetDisplayName(DialogueSpeaker speaker)
    {
        CharacterDefinition character = FindCharacter(speaker);
        return character != null ? character.DisplayName : speaker.ToString();
    }

    public bool TryGetPortrait(
        DialogueSpeaker speaker,
        CharacterExpression expression,
        out Sprite portrait)
    {
        CharacterDefinition character = FindCharacter(speaker);

        if (character?.Expressions != null)
        {
            foreach (ExpressionSprite candidate in character.Expressions)
            {
                if (candidate != null && candidate.Expression == expression)
                {
                    portrait = candidate.Sprite;
                    return portrait != null;
                }
            }
        }

        portrait = null;
        return false;
    }

    private CharacterDefinition FindCharacter(DialogueSpeaker speaker)
    {
        if (characters == null)
        {
            return null;
        }

        foreach (CharacterDefinition character in characters)
        {
            if (character != null && character.Speaker == speaker)
            {
                return character;
            }
        }

        return null;
    }
}
