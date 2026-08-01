using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SceneManagement;

/// <summary>
/// GameCore上でコンテンツシーンのロード、開始同期、閉幕、アンロードを統括します。
/// 各コンテンツシーンは次のシーンを知らず、<see cref="ISceneFlowPort"/>だけを使用します。
/// </summary>
public sealed class GameFlowController
{
    private readonly GameFlowDefinition _definition;
    private readonly ISceneLoader _sceneLoader;
    private readonly SceneFlowSignaler _signaler;
    private readonly ClapStageRunContextStore _stageRunContextStore;
    private readonly CurtainController _curtain;
    private readonly StageSignView _stageSign;
    private readonly GameSession _session;

    /// <summary>GameCoreが所有する遷移サービスと演出Viewからフロー制御を構成します。</summary>
    public GameFlowController(
        GameFlowDefinition definition,
        ISceneLoader sceneLoader,
        SceneFlowSignaler signaler,
        ClapStageRunContextStore stageRunContextStore,
        CurtainController curtain,
        StageSignView stageSign,
        GameSession session)
    {
        _definition = definition;
        _sceneLoader = sceneLoader;
        _signaler = signaler;
        _stageRunContextStore = stageRunContextStore;
        _curtain = curtain;
        _stageSign = stageSign;
        _session = session;
    }

    /// <summary>
    /// <see cref="GameFlowDefinition"/>に登録された順序でゲーム全体を実行します。
    /// </summary>
    public async UniTask RunAsync(CancellationToken cancellationToken)
    {
        if (_definition.Steps.Count == 0)
        {
            throw new InvalidOperationException(
                "GameFlowDefinitionにシーンが登録されていません。");
        }

        _curtain.ResetCurtainPosition();

        do
        {
            _session.Reset();

            foreach (GameFlowDefinition.Step step in _definition.Steps)
            {
                await RunStepAsync(step, cancellationToken);
            }
        }
        while (_definition.Loop && !cancellationToken.IsCancellationRequested);
    }

    private async UniTask RunStepAsync(
        GameFlowDefinition.Step step,
        CancellationToken cancellationToken)
    {
        SceneId sceneId = step.SceneId;
        ClapStageRunContext stageRunContext = step.StageData != null
            ? new ClapStageRunContext(sceneId, step.StageData)
            : null;

        ValidateStep(step);
        bool signalSessionStarted = false;
        bool stageContextStarted = false;

        try
        {
            _signaler.Begin(sceneId);
            signalSessionStarted = true;

            if (stageRunContext != null)
            {
                _stageRunContextStore.Begin(stageRunContext);
                stageContextStarted = true;
            }

            UniTask loadTask = _sceneLoader.LoadAdditiveAsync(
                sceneId,
                cancellationToken);

            UniTask signEnterTask = step.StageData != null
                ? _stageSign.EnterAsync(
                    step.StageData.StageSignSprite,
                    cancellationToken)
                : UniTask.CompletedTask;

            await loadTask;
            await _signaler.WaitUntilReadyAsync(sceneId, cancellationToken);
            await signEnterTask;

            if (step.OpenCurtainBeforeEnter)
            {
                await UniTask.WhenAll(
                    _curtain.OpenCurtainAsync(),
                    step.StageData != null
                        ? _stageSign.ExitAsync(cancellationToken)
                        : UniTask.CompletedTask);
            }
            else if (step.StageData != null)
            {
                await _stageSign.ExitAsync(cancellationToken);
            }

            _signaler.AllowEnter(sceneId);
            await _signaler.WaitUntilCompleteAsync(sceneId, cancellationToken);

            if (step.CloseCurtainOnComplete)
            {
                await _curtain.CloseCurtainAsync();
            }

            await _sceneLoader.UnloadAsync(sceneId, cancellationToken);
        }
        finally
        {
            if (_sceneLoader.IsLoaded(sceneId))
            {
                if (step.CloseCurtainOnComplete)
                {
                    await _curtain.CloseCurtainAsync();
                }

                await _sceneLoader.UnloadAsync(
                    sceneId,
                    CancellationToken.None);
            }

            _stageSign.Hide();
            if (stageContextStarted)
            {
                _stageRunContextStore.End(stageRunContext);
            }

            if (signalSessionStarted)
            {
                _signaler.End(sceneId);
            }
        }
    }

    private static void ValidateStep(GameFlowDefinition.Step step)
    {
        bool isClapStage = step.SceneId.IsClapStage();

        if (isClapStage && step.StageData == null)
        {
            throw new InvalidOperationException(
                $"{step.SceneId}にはClapStageDataの設定が必要です。");
        }

        if (!isClapStage && step.StageData != null)
        {
            throw new InvalidOperationException(
                $"{step.SceneId}は拍手ステージではないため、ClapStageDataを設定できません。");
        }
    }
}
