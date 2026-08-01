using System.Threading;
using Cysharp.Threading.Tasks;
using SceneManagement;
using VContainer.Unity;

public sealed class ClapGameplayDevFlow : IAsyncStartable
{
    private readonly ClapStageSequence _stageSequence;
    private readonly ClapStageStandaloneSettings _standaloneSettings;
    private readonly IClapStageRunContextProvider _stageRunContextProvider;
    private readonly ISceneFlowPort _sceneFlow;
    private readonly GameSession _session;

    public ClapGameplayDevFlow(
        ClapStageSequence stageSequence,
        ClapStageStandaloneSettings standaloneSettings,
        IClapStageRunContextProvider stageRunContextProvider,
        ISceneFlowPort sceneFlow,
        GameSession session)
    {
        _stageSequence = stageSequence;
        _standaloneSettings = standaloneSettings;
        _stageRunContextProvider = stageRunContextProvider;
        _sceneFlow = sceneFlow;
        _session = session;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        ClapRoundResult result;

        if (_stageRunContextProvider.TryGetCurrent(
                out ClapStageRunContext runContext))
        {
            SceneId sceneId = runContext.SceneId;
            if (!_sceneFlow.IsManaged(sceneId))
            {
                throw new System.InvalidOperationException(
                    $"{sceneId}のステージ実行設定は存在しますが、"
                    + "対応するSceneFlowセッションがありません。");
            }

            _stageSequence.PrepareStage();
            _sceneFlow.NotifyReady(sceneId);
            await _sceneFlow.WaitForEnterAsync(sceneId, cancellationToken);

            result = await _stageSequence.PlayContentAsync(
                runContext.StageData,
                cancellationToken);

            _session.Record(result);
            _sceneFlow.NotifyComplete(sceneId);
            return;
        }

        result = await _stageSequence.PlayAsync(
            _standaloneSettings.StageData,
            cancellationToken);
        _session.Record(result);
    }
}
