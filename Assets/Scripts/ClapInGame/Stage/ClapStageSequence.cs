using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class ClapStageSequence
{
    private readonly IClapStageSceneView _sceneView;
    private readonly IStageDialoguePlayer _dialoguePlayer;
    private readonly ICountdownPlayer _countdownPlayer;
    private readonly ClapPresenter _presenter;
    private readonly CurtainController _curtain;

    public ClapStageSequence(
        IClapStageSceneView sceneView,
        IStageDialoguePlayer dialoguePlayer,
        ICountdownPlayer countdownPlayer,
        ClapPresenter presenter,
        CurtainController curtain)
    {
        _sceneView = sceneView;
        _dialoguePlayer = dialoguePlayer;
        _countdownPlayer = countdownPlayer;
        _presenter = presenter;
        _curtain = curtain;
    }

    public async UniTask<ClapRoundResult> PlayAsync(
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        _sceneView.PrepareStage();

        await UniTask.WhenAll(
            _curtain.OpenCurtainAsync(),
            _sceneView.PlayStageSignAsync(cancellationToken));

        await _dialoguePlayer.PlayAsync(stageData.Dialogue, cancellationToken);
        await _countdownPlayer.PlayAsync(cancellationToken);
        await _sceneView.PlayStartCutInAsync(cancellationToken);

        ClapRoundResult result = await _presenter.PlayAsync(stageData, cancellationToken);

        await _sceneView.PlayFinishCutInAsync(cancellationToken);
        await _sceneView.ShowResultAsync(result, stageData, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        await _curtain.CloseCurtainAsync();

        return result;
    }
}
