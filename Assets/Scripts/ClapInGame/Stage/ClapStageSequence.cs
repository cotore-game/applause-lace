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

    /// <summary>ステージ内演出とGameCore演出の依存関係を受け取ります。</summary>
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

    /// <summary>
    /// Devシーン直接起動用に、看板・開幕から閉幕までステージ全体を実行します。
    /// </summary>
    public async UniTask<ClapRoundResult> PlayAsync(
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        PrepareStage();
        _curtain.ResetCurtainPosition();

        await _stageSignView.EnterAsync(
            stageData.StageSignSprite,
            cancellationToken);

        await UniTask.WhenAll(
            _curtain.OpenCurtainAsync(),
            _stageSignView.ExitAsync(cancellationToken));

        ClapRoundResult result = await PlayContentAsync(
            stageData,
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        await _curtain.CloseCurtainAsync();

        return result;
    }

    /// <summary>各Viewを初期状態へ戻し、GameFlowへReady通知できる状態にします。</summary>
    public void PrepareStage()
    {
        _sceneView.PrepareStage();
    }

    /// <summary>
    /// 開幕後のADV、カウントダウン、ゲーム本編、終了演出を実行します。
    /// 幕とStageSignはGameCore側の責務として扱います。
    /// </summary>
    public async UniTask<ClapRoundResult> PlayContentAsync(
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
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

        ClapRoundResult result = await _presenter.PlayAsync(
            stageData,
            cancellationToken);

        await _sceneView.PlayFinishCutInAsync(cancellationToken);
        await _sceneView.ShowResultAsync(result, stageData, cancellationToken);

        return result;
    }
}
