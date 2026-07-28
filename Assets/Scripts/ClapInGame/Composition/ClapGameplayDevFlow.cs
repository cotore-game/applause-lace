using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

public sealed class ClapGameplayDevFlow : IAsyncStartable
{
    private readonly ClapStageData _stageData;
    private readonly ClapStageSequence _stageSequence;

    public ClapGameplayDevFlow(
        ClapStageData stageData,
        ClapStageSequence stageSequence)
    {
        _stageData = stageData;
        _stageSequence = stageSequence;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        await _stageSequence.PlayAsync(_stageData, cancellationToken);
    }
}
