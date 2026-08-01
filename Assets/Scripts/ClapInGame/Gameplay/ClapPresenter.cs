using System;
using System.Threading;
using Cysharp.Threading.Tasks;

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

    public async UniTask<ClapRoundResult> PlayAsync(
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        _view.BeginGameplay();
        _view.OnClapButtonClicked += OnClapRequested;

        try
        {
            return await _manager.StartGameAsync(stageData, cancellationToken);
        }
        finally
        {
            _view.OnClapButtonClicked -= OnClapRequested;
            _view.EndGameplay();
        }
    }

    private void OnClapRequested()
    {
        if (_manager.TryClap())
        {
            _view.UpdateClapCount(_model.ClapCount);
            _view.PlayClapEffect();
        }
    }

    public void Dispose()
    {
        _view.OnClapButtonClicked -= OnClapRequested;
    }
}
