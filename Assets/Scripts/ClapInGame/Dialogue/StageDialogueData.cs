using System;
using UnityEngine;

public enum DialogueSpeaker
{
    Narrator,
    Host,
    Performer
}

public enum CharacterExpression
{
    Neutral,
    Smile,
    Excited,
    Confused,
    Surprised
}

public enum DialogueEventType
{
    None,
    ShowClapButton,
    SpotlightClapButton,
    HideClapButton,
    PlayConfetti,
    Custom
}

public enum DialogueCsvColumn
{
    Order,
    Speaker,
    Expression,
    Text,
    Event,
    EventArgument
}

public readonly struct DialogueLine
{
    public DialogueLine(
        int order,
        DialogueSpeaker speaker,
        CharacterExpression expression,
        string text,
        DialogueEventType eventType,
        string eventArgument)
    {
        Order = order;
        Speaker = speaker;
        Expression = expression;
        Text = text;
        EventType = eventType;
        EventArgument = eventArgument;
    }

    public int Order { get; }
    public DialogueSpeaker Speaker { get; }
    public CharacterExpression Expression { get; }
    public string Text { get; }
    public DialogueEventType EventType { get; }
    public string EventArgument { get; }
}

[CreateAssetMenu(fileName = "StageDialogueData", menuName = "Game/Dialogue/StageDialogueData")]
public sealed class StageDialogueData : ScriptableObject
{
    [SerializeField] private TextAsset csvFile;
    [SerializeField] private CharacterPortraitCatalog portraitCatalog;

    public TextAsset CsvFile => csvFile;
    public CharacterPortraitCatalog PortraitCatalog => portraitCatalog;
}
