using SceneManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CurtainController curtainController;
    [SerializeField] private StageSignView stageSignView;

    [Header("Game Flow")]
    [SerializeField] private SceneCatalog sceneCatalog;
    [SerializeField] private GameFlowDefinition gameFlowDefinition;
    [SerializeField] private bool autoStartGameFlow;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(curtainController);
        builder.RegisterComponent(stageSignView);

        builder.Register<SceneFlowSignaler>(Lifetime.Singleton)
            .AsSelf()
            .As<ISceneFlowPort>();
        builder.Register<GameSession>(Lifetime.Singleton);

        if (sceneCatalog == null || gameFlowDefinition == null)
        {
            if (autoStartGameFlow)
            {
                Debug.LogWarning(
                    "GameFlowの自動開始にはSceneCatalogとGameFlowDefinitionが必要です。",
                    this);
            }

            return;
        }

        builder.RegisterInstance(sceneCatalog);
        builder.RegisterInstance(gameFlowDefinition);
        builder.Register<SceneLoader>(Lifetime.Singleton)
            .As<ISceneLoader>();
        builder.Register<GameFlowController>(Lifetime.Singleton);

        if (autoStartGameFlow)
        {
            builder.RegisterEntryPoint<GameFlowEntryPoint>();
        }
    }
}
