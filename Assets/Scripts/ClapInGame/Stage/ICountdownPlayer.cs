using System.Threading;
using Cysharp.Threading.Tasks;

public interface ICountdownPlayer
{
    UniTask PlayAsync(CancellationToken cancellationToken);
}

public sealed class NullCountdownPlayer : ICountdownPlayer
{
    public UniTask PlayAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }
}
