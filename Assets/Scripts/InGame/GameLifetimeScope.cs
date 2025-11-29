using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameView gameView;
    [SerializeField] private StageData[] stageList; // Inspector

    protected override void Configure(IContainerBuilder builder)
    {
        // Model
        builder.Register<ClapGameModel>(Lifetime.Scoped);

        // View
        builder.RegisterComponent(gameView);

        // Data
        builder.RegisterInstance(stageList);

        // Presenter
        builder.RegisterEntryPoint<GamePresenter>();

        // Flow
        builder.RegisterEntryPoint<GameFlowController>();

        // 会話制御など
        builder.Register<TutorialService>(Lifetime.Scoped);
    }
}
