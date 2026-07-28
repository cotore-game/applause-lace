using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IDialogueEventDispatcher
{
    UniTask DispatchAsync(
        DialogueEventType eventType,
        string eventArgument,
        CancellationToken cancellationToken);
}

public sealed class DialogueEventDispatcher : IDialogueEventDispatcher
{
    public UniTask DispatchAsync(
        DialogueEventType eventType,
        string eventArgument,
        CancellationToken cancellationToken)
    {
        if (eventType != DialogueEventType.None)
        {
            Debug.LogWarning(
                $"ADVイベント '{eventType}' はまだハンドラーが未登録です。argument='{eventArgument}'");
        }

        return UniTask.CompletedTask;
    }
}
