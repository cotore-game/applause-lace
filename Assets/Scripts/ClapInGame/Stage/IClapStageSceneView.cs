using System.Threading;
using Cysharp.Threading.Tasks;

public interface IClapStageSceneView
{
    void PrepareStage();
    UniTask PlayStageSignAsync(CancellationToken cancellationToken);
    UniTask PlayStartCutInAsync(CancellationToken cancellationToken);
    UniTask PlayFinishCutInAsync(CancellationToken cancellationToken);
    UniTask ShowResultAsync(
        ClapRoundResult result,
        ClapStageData stageData,
        CancellationToken cancellationToken);
}

public sealed class NullClapStageSceneView : IClapStageSceneView
{
    public void PrepareStage()
    {
    }

    public UniTask PlayStageSignAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public UniTask PlayStartCutInAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public UniTask PlayFinishCutInAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public UniTask ShowResultAsync(
        ClapRoundResult result,
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }
}
