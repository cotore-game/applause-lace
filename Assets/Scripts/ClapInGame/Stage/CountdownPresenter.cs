using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class CountdownPresenter : ICountdownPlayer
{
    private readonly CountdownView _view;

    public CountdownPresenter(CountdownView view)
    {
        _view = view;
    }

    public async UniTask PlayCountsAsync(CancellationToken cancellationToken)
    {
        try
        {
            for (int count = 3; count >= 1; count--)
            {
                _view.ShowCount(count);
                await DelayAsync(
                    _view.CountIntervalSeconds,
                    cancellationToken);
            }
        }
        catch
        {
            _view.Hide();
            throw;
        }
    }

    public async UniTask PlayCelebrateAsync(CancellationToken cancellationToken)
    {
        try
        {
            _view.ShowCelebrate();
            await DelayAsync(
                _view.CelebrateSeconds,
                cancellationToken);
        }
        finally
        {
            _view.Hide();
        }
    }

    private static UniTask DelayAsync(
        float seconds,
        CancellationToken cancellationToken)
    {
        return seconds > 0f
            ? UniTask.Delay(
                TimeSpan.FromSeconds(seconds),
                cancellationToken: cancellationToken)
            : UniTask.CompletedTask;
    }
}
