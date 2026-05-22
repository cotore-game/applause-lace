using VContainer;
using VContainer.Unity;
using UnityEngine;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CurtainController curtainController;

    protected override void Configure(IContainerBuilder builder)
    {
        // 幕のインスタンスをプロジェクト全体（常駐スコープ）に登録
        builder.RegisterComponent(curtainController);
    }
}
