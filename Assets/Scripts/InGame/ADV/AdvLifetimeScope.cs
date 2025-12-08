using VContainer;
using VContainer.Unity;
using UnityEngine;
using ADV.Presentation;
using ADV.Commands;
using SceneManagement;

namespace ADV.System
{
    /// <summary>
    /// ADVシーン用のLifetimeScope
    /// ADVシナリオ実行に必要な依存関係を登録
    /// </summary>
    public class AdvLifetimeScope : LifetimeScope
    {
        [Header("View Prefabs")]
        [SerializeField] private TextDisplayView textView;
        [SerializeField] private CharacterView characterView;
        [SerializeField] private Transform characterContainer;

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLog = true;
        [SerializeField] private TextAsset defaultScenarioData = null;

        protected override void Configure(IContainerBuilder builder)
        {
            // View層の登録
            builder.RegisterComponent(textView);

            // Presenter層の登録
            builder.Register<TextPresenter>(Lifetime.Scoped)
                .WithParameter(textView);

            builder.Register<CharacterPresenter>(Lifetime.Scoped)
                .WithParameter(characterContainer)
                .WithParameter(characterView);

            // SceneTransitionerの登録
            builder.RegisterInstance(SceneTransitioner.Instance);

            // CommandDependenciesの登録
            builder.Register<CommandDependencies>(Lifetime.Scoped);

            // CommandFactoryの登録
            builder.Register<CommandFactory>(Lifetime.Scoped);

            // AdvScenarioExecutorの登録
            builder.RegisterEntryPoint<AdvScenarioExecutor>()
                .WithParameter(enableDebugLog)
                .WithParameter(defaultScenarioData);
        }
    }
}
