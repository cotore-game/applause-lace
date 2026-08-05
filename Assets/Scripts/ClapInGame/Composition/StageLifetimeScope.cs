using UnityEngine;
using VContainer;
using VContainer.Unity;
using SceneManagement;
using UnityEngine.Serialization;

public class StageLifetimeScope : LifetimeScope
{
    [Header("Standalone Play Only")]
    [FormerlySerializedAs("stageData")]
    [SerializeField] private ClapStageData standaloneStageData;
    [FormerlySerializedAs("sceneId")]
    [SerializeField] private SceneId standaloneSceneId = SceneId.Stage1;

    [Header("Prefab Views")]
    [SerializeField] private ClapStageView stageView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance(new ClapStageStandaloneSettings(
            standaloneSceneId,
            standaloneStageData));
        builder.RegisterComponent(stageView)
            .AsSelf()
            .As<IClapStageSceneView>()
            .As<IClapGameplayView>();

        if (stageView.Countdown != null)
        {
            builder.RegisterComponent(stageView.Countdown);
            builder.Register<CountdownPresenter>(Lifetime.Scoped)
                .As<ICountdownPlayer>();
        }
        else
        {
            builder.Register<NullCountdownPlayer>(Lifetime.Scoped)
                .As<ICountdownPlayer>();
        }

        if (stageView.DialogueEvents != null)
        {
            builder.RegisterComponent(stageView.DialogueEvents)
                .As<IDialogueEventDispatcher>();
        }
        else
        {
            builder.Register<DialogueEventDispatcher>(Lifetime.Scoped)
                .As<IDialogueEventDispatcher>();
        }

        if (stageView.Dialogue != null)
        {
            builder.RegisterComponent(stageView.Dialogue);
            builder.Register<StageDialoguePresenter>(Lifetime.Scoped)
                .As<IStageDialoguePlayer>();
        }
        else
        {
            builder.Register<NullStageDialoguePlayer>(Lifetime.Scoped)
                .As<IStageDialoguePlayer>();
        }

        builder.Register<ClapGameModel>(Lifetime.Scoped);
        builder.Register<ClapGameplayManager>(Lifetime.Scoped);
        builder.Register<ClapPresenter>(Lifetime.Scoped);
        builder.Register<ClapStageSequence>(Lifetime.Scoped);
        builder.RegisterEntryPoint<ClapGameplayDevFlow>();
    }
}
