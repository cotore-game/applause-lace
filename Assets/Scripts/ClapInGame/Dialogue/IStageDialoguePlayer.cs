using System.Threading;
using Cysharp.Threading.Tasks;

public interface IStageDialoguePlayer
{
    UniTask PlayAsync(
        StageDialogueData dialogueData,
        CancellationToken cancellationToken);
}

public sealed class NullStageDialoguePlayer : IStageDialoguePlayer
{
    public UniTask PlayAsync(
        StageDialogueData dialogueData,
        CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }
}
