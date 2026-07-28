using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ClapPresenter : IDisposable
{
    private readonly ClapGameplayManager _manager;
    private readonly ClapGameModel _model;
    private readonly ClapStageView _view;

    public ClapPresenter(ClapGameplayManager manager, ClapGameModel model, ClapStageView view)
    {
        _manager = manager;
        _model = model;
        _view = view;
    }

    public async UniTask<ClapRoundResult> PlayAsync(CancellationToken cancellationToken)
    {
        _view.ResetView();
        _view.OnClapButtonClicked += OnClapRequested;

        ClapRoundResult result = await _manager.StartGameAsync(cancellationToken);

        if (result.IsGameOver)
        {
            _view.ShowResult(
                $"GAME OVER\nClap: {result.RawClapCount}\nScore: 0",
                Color.red);
        }
        else
        {
            _view.ShowResult(
                $"TIME UP!\nClap: {result.RawClapCount}",
                Color.green);
        }

        return result;
    }

    private void OnClapRequested()
    {
        if (_manager.TryClap())
        {
            _view.UpdateClapCount(_model.ClapCount);
        }
    }

    public void Dispose()
    {
        if (_view != null)
        {
            _view.OnClapButtonClicked -= OnClapRequested;
        }
    }
}
