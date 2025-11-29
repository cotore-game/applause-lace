using VContainer;
using VContainer.Unity;
using System;

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
        _view.OnClickAction = () => _model.Click();

        // Model -> View
        _model.OnScoreChanged += score => _view.UpdateScore(score);
        _model.OnConfettiRequested += () => _view.PlayConfetti();
    }

    public void Dispose()
    {
        _model.OnScoreChanged -= score => _view.UpdateScore(score);
    }
}
