using System;
using System.Collections.Generic;
using CSV4Unity;

public static class DialogueCsvReader
{
    public static IReadOnlyList<DialogueLine> Read(StageDialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.CsvFile == null)
        {
            return Array.Empty<DialogueLine>();
        }

        CsvData<DialogueCsvColumn> csv =
            CSVLoader.LoadCSV<DialogueCsvColumn>(dialogueData.CsvFile);
        var lines = new List<DialogueLine>(csv.Rows.Count);

        foreach (LineData<DialogueCsvColumn> row in csv.Rows)
        {
            int order = row.GetOrDefault(DialogueCsvColumn.Order, lines.Count + 1);
            DialogueSpeaker speaker = ParseEnum(
                row.GetOrDefault<string>(DialogueCsvColumn.Speaker),
                DialogueSpeaker.Narrator,
                DialogueCsvColumn.Speaker,
                order);
            CharacterExpression expression = ParseEnum(
                row.GetOrDefault<string>(DialogueCsvColumn.Expression),
                CharacterExpression.Neutral,
                DialogueCsvColumn.Expression,
                order);
            DialogueEventType eventType = ParseEnum(
                row.GetOrDefault<string>(DialogueCsvColumn.Event),
                DialogueEventType.None,
                DialogueCsvColumn.Event,
                order);

            lines.Add(new DialogueLine(
                order,
                speaker,
                expression,
                row.GetOrDefault<string>(DialogueCsvColumn.Text) ?? string.Empty,
                eventType,
                row.GetOrDefault<string>(DialogueCsvColumn.EventArgument) ?? string.Empty));
        }

        lines.Sort((left, right) => left.Order.CompareTo(right.Order));
        return lines;
    }

    private static TEnum ParseEnum<TEnum>(
        string value,
        TEnum defaultValue,
        DialogueCsvColumn column,
        int order)
        where TEnum : struct
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse(value, true, out TEnum parsed))
        {
            return parsed;
        }

        throw new FormatException(
            $"Dialogue CSVの{order}行目: {column}='{value}' は未定義です。");
    }
}
