using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

public class ClapPresenter : IAsyncStartable, ITickable, IDisposable
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

    public async UniTask StartAsync(CancellationToken cancellationToken)
    {
        _view.ResetView();
        _view.OnStopButtonClicked += OnStopRequested;

        // ゲームループ開始
        bool isSuccess = await _manager.StartGameAsync(cancellationToken);

        // ループを抜けたら（ゲームが終了したら）結果を表示
        if (_model.IsGameOver)
        {
            _view.ShowResult("GAME OVER\n(Time Out!)", Color.red);
        }
        else if (isSuccess)
        {
            _view.ShowResult($"STAGE CLEAR!\nClap: {_model.ClapCount}", Color.green);
        }
    }

    /// <summary>
    /// 毎フレーム更新される処理。ゲームが終了していない場合、最新の拍手回数をViewに反映する。
    /// </summary>
    public void Tick()
    {
        if (_model == null || _model.IsGameOver || _model.IsSuccess) return;

        // 毎フレーム、最新の拍手回数をViewに反映
        _view.UpdateClapCount(_model.ClapCount);
    }

    /// <summary>
    /// ストップ処理
    /// </summary>
    private void OnStopRequested()
    {
        _manager.ForceSuccess();
    }

    public void Dispose()
    {
        if (_view != null)
        {
            _view.OnStopButtonClicked -= OnStopRequested;
        }
    }
}
