using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

/// <summary>VContainerの起動処理から全体GameFlowを開始します。</summary>
public sealed class GameFlowEntryPoint : IAsyncStartable
{
    private readonly GameFlowController _controller;

    /// <summary>起動対象となるGameFlowControllerを受け取ります。</summary>
    public GameFlowEntryPoint(GameFlowController controller)
    {
        _controller = controller;
    }

    /// <inheritdoc />
    public UniTask StartAsync(CancellationToken cancellationToken)
    {
        return _controller.RunAsync(cancellationToken);
    }
}
