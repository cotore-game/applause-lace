using VContainer;
using VContainer.Unity;
using UnityEngine;
using GameDialogue;

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
        builder.RegisterEntryPoint<GamePlayController>();

        // 会話制御など
        builder.Register<DialogueSystem>(Lifetime.Scoped);
    }
}
