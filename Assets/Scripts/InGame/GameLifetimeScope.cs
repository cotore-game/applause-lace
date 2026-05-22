using VContainer;
using VContainer.Unity;
using UnityEngine;
using GameDialogue;
using ADV.Presentation;
using ADV.Commands;
using ADV.System;
using SceneManagement;

public class GameLifetimeScope : LifetimeScope
{
    // --- GameLifetimeScopeからのSerializeField ---
    [Header("Game Settings")]
    [SerializeField] private GameView gameView;
    [SerializeField] private StageData[] stageList;

    // --- 追加のView ---
    [Header("Additional Views")]
    [SerializeField] private TitleView titleView;
    [SerializeField] private ResultView resultView;
    [SerializeField] private CountdownView countdownView;

    // --- AdvLifetimeScopeからのSerializeField ---
    [Header("ADV View Prefabs")]
    [SerializeField] private TextDisplayView textView;
    [SerializeField] private CharacterView characterView;

    [Header("ADV Debug Settings")]
    [SerializeField] private bool enableDebugLog = true;
    [SerializeField] private TextAsset defaultScenarioData = null;

    protected override void Configure(IContainerBuilder builder)
    {

        // Model (ClapGameModel)
        builder.Register<ClapGameModel>(Lifetime.Scoped);

        // View (GameView)
        builder.RegisterComponent(gameView);

        builder.RegisterComponent(titleView);
        builder.RegisterComponent(resultView);
        builder.RegisterComponent(countdownView);

        // Data (StageData[])
        builder.RegisterInstance(stageList);

        // EntryPoint - Presenter (GamePresenter)
        builder.RegisterEntryPoint<GamePresenter>();

        // EntryPoint - Flow (GamePlayController)
        builder.RegisterEntryPoint<GamePlayController>();

        // View層
        builder.RegisterComponent(textView);

        // Presenter層
        builder.Register<TextPresenter>(Lifetime.Scoped)
            .WithParameter(textView);

        builder.Register<CharacterPresenter>(Lifetime.Scoped)
            .WithParameter(characterView);

        // SceneTransitioner (Singleton Instance)
        builder.RegisterInstance(SceneTransitioner.Instance);

        // CommandFactory
        builder.Register<CommandFactory>(Lifetime.Scoped);
    }
}
