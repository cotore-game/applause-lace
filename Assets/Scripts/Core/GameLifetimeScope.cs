using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CurtainController curtainController;
    [SerializeField] private StageSignView stageSignView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(curtainController);
        builder.RegisterComponent(stageSignView);
    }
}
