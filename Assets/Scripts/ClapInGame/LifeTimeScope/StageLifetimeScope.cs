using VContainer;
using VContainer.Unity;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class StageLifetimeScope : LifetimeScope
{
    [SerializeField] private ClapStageData stageData;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(stageData);
        builder.Register<ClapGameModel>(Lifetime.Scoped);
        builder.Register<ClapGameplayManager>(Lifetime.Scoped);

        // IAsyncStartableとして登録
        builder.RegisterEntryPoint<StageTestEntryPoint>();
    }
}

public class StageTestEntryPoint : IAsyncStartable
{
    private readonly ClapGameplayManager _manager;

    public StageTestEntryPoint(ClapGameplayManager manager)
    {
        _manager = manager;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        // スコープ破棄と連動したtokenをそのまま渡す
        await _manager.StartGameAsync(cancellationToken);
    }
}
