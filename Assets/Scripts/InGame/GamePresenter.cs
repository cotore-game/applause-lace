using VContainer;
using VContainer.Unity;
using System;
using Cysharp.Threading.Tasks;

public class GamePresenter : IStartable, IDisposable
{
    private readonly ClapGameModel _model;
    private readonly GameView _view;

    [Inject]
    public GamePresenter(ClapGameModel model, GameView view)
    {
        _model = model;
        _view = view;
    }

    public void Start()
    {
        // View -> Model
        _view.OnClapAction += HandleClap;

        // Model -> View
        _model.OnScoreChanged += _view.UpdateScore;
        _model.OnConfettiRequested += _view.PlayConfetti;
        _model.OnFeverStarted += OnFeverStarted;
        _model.OnHiddenTimeUp += OnTimeUp;
        _model.OnRoundFailed += OnRoundFailed;
    }

    public void Dispose()
    {
        _view.OnClapAction -= HandleClap;
        _model.OnScoreChanged -= _view.UpdateScore;
        _model.OnConfettiRequested -= _view.PlayConfetti;
        _model.OnFeverStarted -= OnFeverStarted;
        _model.OnHiddenTimeUp -= OnTimeUp;
        _model.OnRoundFailed -= OnRoundFailed;
    }

    private void HandleClap()
    {
        _model.Click();
    }

    private async void OnFeverStarted()
    {
        await UniTask.Yield();
        // フィーバー開始時の演出
    }

    private async void OnTimeUp()
    {
        await UniTask.Yield();
        // タイムアップ時の演出
    }

    private async void OnRoundFailed()
    {
        _view.StopClapEffects();
        await _view.HideSpotlight();
    }
}
