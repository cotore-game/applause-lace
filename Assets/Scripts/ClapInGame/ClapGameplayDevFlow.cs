using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

public sealed class ClapGameplayDevFlow : IAsyncStartable
{
    private readonly CurtainController _curtain;
    private readonly ClapPresenter _presenter;

    public ClapGameplayDevFlow(
        CurtainController curtain,
        ClapPresenter presenter)
    {
        _curtain = curtain;
        _presenter = presenter;
    }

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        await _curtain.OpenCurtainAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await _presenter.PlayAsync(cancellationToken);
    }
}
