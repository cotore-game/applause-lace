using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ClapGameplayManager
{
    private readonly ClapGameModel _model;
    private readonly ClapStageData _stageData;

    // 今回のプレイで決定された動的な時間
    private float _currentTargetTime;
    private float _currentLimitTime;

    public ClapGameplayManager(ClapGameModel model, ClapStageData stageData)
    {
        _model = model;
        _stageData = stageData;
    }

    public async UniTask<bool> StartGameAsync(CancellationToken token)
    {
        _model.Reset();

        // 乱数で今回のターゲット時間を決定 [baseTime - randomRange, baseTime + randomRange] の範囲
        _currentTargetTime = _stageData.baseTime + Random.Range(-_stageData.randomRange, _stageData.randomRange);
        _currentLimitTime = _currentTargetTime + _stageData.limitOffset;

        Debug.Log($"[ゲーム開始] 基本: {_stageData.baseTime}秒 (ブレ: +-{_stageData.randomRange}秒)");
        Debug.Log($"[今回の目標] ターゲット: {_currentTargetTime:F2}秒 / 限界アウト: {_currentLimitTime:F2}秒");

        while (!_model.IsGameOver && !_model.IsSuccess)
        {
            if (token.IsCancellationRequested) return false;

            _model.ElapsedTime += Time.deltaTime;

            // Input Systemでの入力検知 (Spaceキー または マウス左クリック/画面タップ)
            bool isClapPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                                 (Pointer.current != null && Pointer.current.press.wasPressedThisFrame);

            if (isClapPressed)
            {
                OnClap();
            }

            // プレイヤーが叩かないまま限界時間を超えたら自動アウト
            if (_model.ElapsedTime > _currentLimitTime)
            {
                _model.IsGameOver = true;
                Debug.Log($"[GameOver] 限界時間を超過！ 拍手回数: {_model.ClapCount}");
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        return _model.IsSuccess;
    }

    private void OnClap()
    {
        if (_model.IsGameOver || _model.IsSuccess) return;

        _model.ClapCount++;
        Debug.Log($"[拍手] 回数: {_model.ClapCount} | 経過時間: {_model.ElapsedTime:F2}秒");

        // 限界時間を超えた状態で叩いた場合もアウト
        if (_model.ElapsedTime > _currentLimitTime)
        {
            _model.IsGameOver = true;
            Debug.Log($"[ゲームオーバー] 限界時間 {_currentLimitTime:F2}秒 を過ぎて叩きました。");
        }
    }

    public void ForceSuccess()
    {
        if (_model.IsGameOver) return;
        _model.IsSuccess = true;

        // ターゲット時間からどれだけギリギリを攻められたか計算
        float diff = _currentTargetTime - _model.ElapsedTime;
        Debug.Log($"[ステージクリア] セーフで終了！ 記録: {_model.ClapCount}回 (ターゲットとの差: {diff:F2}秒)");
    }
}
