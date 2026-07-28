using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class StageDialoguePresenter : IStageDialoguePlayer
{
    private readonly DialogueView _view;
    private readonly IDialogueEventDispatcher _eventDispatcher;

    public StageDialoguePresenter(
        DialogueView view,
        IDialogueEventDispatcher eventDispatcher)
    {
        _view = view;
        _eventDispatcher = eventDispatcher;
    }

    public async UniTask PlayAsync(
        StageDialogueData dialogueData,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<DialogueLine> lines = DialogueCsvReader.Read(dialogueData);
        CharacterPortraitCatalog catalog = dialogueData != null
            ? dialogueData.PortraitCatalog
            : null;

        try
        {
            foreach (DialogueLine line in lines)
            {
                string speakerName = catalog != null
                    ? catalog.GetDisplayName(line.Speaker)
                    : line.Speaker.ToString();

                Sprite portrait = null;
                catalog?.TryGetPortrait(line.Speaker, line.Expression, out portrait);

                await _view.ShowLineAsync(
                    speakerName,
                    line.Text,
                    portrait,
                    cancellationToken);

                await _eventDispatcher.DispatchAsync(
                    line.EventType,
                    line.EventArgument,
                    cancellationToken);
            }
        }
        finally
        {
            _view.Hide();
        }
    }
}
