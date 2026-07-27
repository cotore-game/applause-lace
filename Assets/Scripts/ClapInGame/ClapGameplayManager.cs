using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClapGameplayManager
{
    private readonly ClapGameModel _model;
    private readonly ClapStageData _stageData;

    private float _clapDeadline;
    private float _roundEndTime;
    private float _roundStartedAt;

    public ClapGameplayManager(ClapGameModel model, ClapStageData stageData)
    {
        _model = model;
        _stageData = stageData;
    }

    public async UniTask<ClapRoundResult> StartGameAsync(CancellationToken token)
    {
        _model.StartRound();

        _clapDeadline = _stageData.baseTime
                        + Random.Range(-_stageData.randomRange, _stageData.randomRange);
        _roundEndTime = _clapDeadline + _stageData.limitOffset;
        _roundStartedAt = Time.time;

        Debug.Log($"[ゲーム開始] 基本: {_stageData.baseTime}秒 (ブレ: +-{_stageData.randomRange}秒)");
        Debug.Log($"[内部判定] 拍手期限: {_clapDeadline:F2}秒 / ラウンド終了: {_roundEndTime:F2}秒");

        while (_model.Phase == ClapRoundPhase.Playing)
        {
            token.ThrowIfCancellationRequested();

            float elapsedTime = Time.time - _roundStartedAt;
            _model.UpdateElapsedTime(elapsedTime);

            if (elapsedTime >= _roundEndTime)
            {
                _model.FinishRound();
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        ClapRoundResult result = new(
            _stageData.stageNumber,
            _model.ClapCount,
            _model.IsGameOver);

        Debug.Log(
            $"[タイムアップ] 拍手回数: {result.RawClapCount} / "
            + $"ゲームオーバー: {result.IsGameOver} / スコア: {result.Score}");

        return result;
    }

    public bool TryClap()
    {
        if (_model.Phase != ClapRoundPhase.Playing)
        {
            return false;
        }

        float elapsedTime = Time.time - _roundStartedAt;
        _model.UpdateElapsedTime(elapsedTime);

        if (elapsedTime >= _roundEndTime)
        {
            return false;
        }

        bool isOvertime = elapsedTime > _clapDeadline;
        bool wasGameOver = _model.IsGameOver;
        _model.RegisterClap(isOvertime);

        Debug.Log($"[拍手] 回数: {_model.ClapCount} | 経過時間: {elapsedTime:F2}秒");

        if (!wasGameOver && _model.IsGameOver)
        {
            Debug.Log($"[ゲームオーバー確定] 拍手期限 {_clapDeadline:F2}秒を超えて拍手しました。");
        }

        return true;
    }
}
