using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class ClapStageSequence
{
    private readonly IClapStageSceneView _sceneView;
    private readonly IStageDialoguePlayer _dialoguePlayer;
    private readonly ICountdownPlayer _countdownPlayer;
    private readonly ClapPresenter _presenter;
    private readonly CurtainController _curtain;
    private readonly StageSignView _stageSignView;

    public ClapStageSequence(
        IClapStageSceneView sceneView,
        IStageDialoguePlayer dialoguePlayer,
        ICountdownPlayer countdownPlayer,
        ClapPresenter presenter,
        CurtainController curtain,
        StageSignView stageSignView)
    {
        _sceneView = sceneView;
        _dialoguePlayer = dialoguePlayer;
        _countdownPlayer = countdownPlayer;
        _presenter = presenter;
        _curtain = curtain;
        _stageSignView = stageSignView;
    }

    public async UniTask<ClapRoundResult> PlayAsync(
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        _sceneView.PrepareStage();
        _curtain.ResetCurtainPosition();

        await _stageSignView.EnterAsync(
            stageData.StageSignSprite,
            cancellationToken);

        await UniTask.WhenAll(
            _curtain.OpenCurtainAsync(),
            _stageSignView.ExitAsync(cancellationToken));

        await _dialoguePlayer.PlayAsync(stageData.Dialogue, cancellationToken);

        _sceneView.ShowStartCutIn();

        try
        {
            await _countdownPlayer.PlayCountsAsync(cancellationToken);
        }
        finally
        {
            _sceneView.HideStartCutIn();
        }

        await _countdownPlayer.PlayCelebrateAsync(cancellationToken);

        ClapRoundResult result = await _presenter.PlayAsync(stageData, cancellationToken);

        await _sceneView.PlayFinishCutInAsync(cancellationToken);
        await _sceneView.ShowResultAsync(result, stageData, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        await _curtain.CloseCurtainAsync();

        return result;
    }
}
