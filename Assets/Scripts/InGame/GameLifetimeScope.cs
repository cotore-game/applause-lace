using VContainer;
using VContainer.Unity;
using UnityEngine;
using GameDialogue;
using ADV.Presentation;
using ADV.Commands;
using ADV.System; // AdvLifetimeScopeがこの名前空間に属していたため
using SceneManagement;

// 以下のクラス名や参照アセンブリ名は、プロジェクトに合わせて調整してください。
// ADV.System のクラスと GameDialogue のクラスが混在することになります。

public class CompositeLifetimeScope : LifetimeScope
{
    // --- GameLifetimeScopeからのSerializeField ---
    [Header("Game Settings")]
    [SerializeField] private GameView gameView;
    [SerializeField] private StageData[] stageList; // Inspectorで設定するデータ

    // --- AdvLifetimeScopeからのSerializeField ---
    [Header("ADV View Prefabs")]
    [SerializeField] private TextDisplayView textView;
    [SerializeField] private CharacterView characterView;

    [Header("ADV Debug Settings")]
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private TextAsset defaultScenarioData = null;

    protected override void Configure(IContainerBuilder builder)
    {
        // ===========================================
        // 1. GameLifetimeScope の設定
        // ===========================================

        // Model (ClapGameModel)
        builder.Register<ClapGameModel>(Lifetime.Scoped);

        // View (GameView)
        builder.RegisterComponent(gameView);

        // Data (StageData[])
        builder.RegisterInstance(stageList);

        // EntryPoint - Presenter (GamePresenter)
        builder.RegisterEntryPoint<GamePresenter>();

        // EntryPoint - Flow (GamePlayController)
        builder.RegisterEntryPoint<GamePlayController>();

        // ===========================================
        // 2. AdvLifetimeScope の設定
        // ===========================================

        // View層 (TextDisplayView, CharacterView はPresenterで直接使用)
        builder.RegisterComponent(textView);

        // Presenter層 (TextPresenter, CharacterPresenter)
        builder.Register<TextPresenter>(Lifetime.Scoped)
            .WithParameter(textView);

        builder.Register<CharacterPresenter>(Lifetime.Scoped)
            .WithParameter(characterView);

        // SceneTransitioner (Singleton Instance)
        // Note: SceneTransitioner.Instance が存在する前提
        builder.RegisterInstance(SceneTransitioner.Instance);

        // CommandFactory
        builder.Register<CommandFactory>(Lifetime.Scoped);

        // EntryPoint (AdvScenarioExecutor)
        builder.RegisterEntryPoint<AdvScenarioExecutor>()
            .WithParameter(enableDebugLog)
            .WithParameter(defaultScenarioData);
    }
}
