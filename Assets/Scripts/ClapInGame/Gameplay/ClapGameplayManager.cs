using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClapGameplayManager
{
    private readonly ClapGameModel _model;

    private float _clapDeadline;
    private float _roundEndTime;
    private float _roundStartedAt;

    public ClapGameplayManager(ClapGameModel model)
    {
        _model = model;
    }

    public async UniTask<ClapRoundResult> StartGameAsync(
        ClapStageData stageData,
        CancellationToken token)
    {
        _model.StartRound();

        _clapDeadline = stageData.BaseTime
                        + Random.Range(-stageData.RandomRange, stageData.RandomRange);
        _roundEndTime = _clapDeadline + stageData.LimitOffset;
        _roundStartedAt = Time.time;

        Debug.Log($"[ゲーム開始] 基本: {stageData.BaseTime}秒 (ブレ: +-{stageData.RandomRange}秒)");
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
            stageData.StageNumber,
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
