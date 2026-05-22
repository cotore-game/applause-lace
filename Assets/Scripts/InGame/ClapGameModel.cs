using System;
using Cysharp.Threading.Tasks;

public class ClapGameModel
{
    // Viewへの通知用
    public event Action<int> OnScoreChanged; // スコア更新時
    public event Action OnFeverStarted; // フィーバー開始時
    public event Action OnConfettiRequested; // 紙吹雪演出要求
    public event Action OnHiddenTimeUp; // タイムアップ到達
    public event Action OnRoundFailed;

    // State
    public GameState CurrentState { get; private set; } = GameState.Ready;
    public int Score { get; private set; }

    private float _hiddenTimeLimit;
    private float _elapsedTime;

    // タイムアップ判定
    public bool IsTimeUp => _elapsedTime >= _hiddenTimeLimit;

    private int _multiplier = 1;

    public void StartGame(StageData data)
    {
        Score = 0;
        _multiplier = 1;
        _elapsedTime = 0;
        _hiddenTimeLimit = data.Duration;

        CurrentState = GameState.Playing;
        RunTimerAsync().Forget();
    }

    public void Click()
    {
        if (CurrentState != GameState.Playing) return;

        // タイムアップ後のクリックはスコア0にして失敗通知
        if (IsTimeUp)
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
            OnRoundFailed?.Invoke();

            // ラウンド自体はここで終了とする
            CurrentState = GameState.Finished;
            return;
        }

        Score += 1 * _multiplier;
        OnScoreChanged?.Invoke(Score);
        OnConfettiRequested?.Invoke();
    }

    public void Stop()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Finished;
        }
    }

    private async UniTaskVoid RunTimerAsync()
    {
        // タイムアップ後も、プレイヤーがクリックして失敗するか、
        // 成功確定とみなされるまでタイマー自体は回し続ける
        while (CurrentState == GameState.Playing)
        {
            _elapsedTime += UnityEngine.Time.deltaTime;

            if (Math.Abs(_elapsedTime - _hiddenTimeLimit) < UnityEngine.Time.deltaTime)
            {
                OnHiddenTimeUp?.Invoke();
            }

            await UniTask.Yield();
        }
    }
}
