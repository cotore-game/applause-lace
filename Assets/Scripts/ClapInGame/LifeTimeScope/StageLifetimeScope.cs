using VContainer;
using VContainer.Unity;
using UnityEngine;

public class StageLifetimeScope : LifetimeScope
{
    [SerializeField] private ClapStageData stageData;
    [SerializeField] private ClapStageView stageView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(stageData);
        builder.RegisterComponent(stageView);
        builder.Register<ClapGameModel>(Lifetime.Scoped);
        builder.Register<ClapGameplayManager>(Lifetime.Scoped);
        builder.Register<ClapPresenter>(Lifetime.Scoped);
        builder.RegisterEntryPoint<ClapGameplayDevFlow>();
    }
}
