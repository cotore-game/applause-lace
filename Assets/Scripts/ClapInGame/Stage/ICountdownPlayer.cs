using System.Threading;
using Cysharp.Threading.Tasks;

public interface ICountdownPlayer
{
    UniTask PlayCountsAsync(CancellationToken cancellationToken);
    UniTask PlayCelebrateAsync(CancellationToken cancellationToken);
}

public sealed class NullCountdownPlayer : ICountdownPlayer
{
    public UniTask PlayCountsAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public UniTask PlayCelebrateAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }
}
