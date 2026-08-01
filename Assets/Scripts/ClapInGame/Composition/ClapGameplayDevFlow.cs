using System.Threading;
using Cysharp.Threading.Tasks;
using SceneManagement;
using VContainer.Unity;

public sealed class ClapGameplayDevFlow : IAsyncStartable
{
    private readonly ClapStageData _stageData;
    private readonly ClapStageSequence _stageSequence;
    private readonly SceneIdentity _sceneIdentity;
    private readonly ISceneFlowPort _sceneFlow;
    private readonly GameSession _session;

    public ClapGameplayDevFlow(
        ClapStageData stageData,
        ClapStageSequence stageSequence,
        SceneIdentity sceneIdentity,
        ISceneFlowPort sceneFlow,
        GameSession session)
    {
        _stageData = stageData;
        _stageSequence = stageSequence;
        _sceneIdentity = sceneIdentity;
        _sceneFlow = sceneFlow;
        _session = session;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        SceneId sceneId = _sceneIdentity.Id;
        ClapRoundResult result;

        if (_sceneFlow.IsManaged(sceneId))
        {
            _stageSequence.PrepareStage();
            _sceneFlow.NotifyReady(sceneId);
            await _sceneFlow.WaitForEnterAsync(sceneId, cancellationToken);

            result = await _stageSequence.PlayContentAsync(
                _stageData,
                cancellationToken);

            _session.Record(result);
            _sceneFlow.NotifyComplete(sceneId);
            return;
        }

        result = await _stageSequence.PlayAsync(_stageData, cancellationToken);
        _session.Record(result);
    }
}
