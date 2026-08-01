using System.Threading;
using Cysharp.Threading.Tasks;

public interface IClapStageSceneView
{
    void PrepareStage();
    void ShowStartCutIn();
    void HideStartCutIn();
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

    public void ShowStartCutIn()
    {
    }

    public void HideStartCutIn()
    {
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
