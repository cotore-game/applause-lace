using System.Collections.Generic;

/// <summary>GameCoreの生存中、各ステージの結果を総合リザルトまで保持します。</summary>
public sealed class GameSession
{
    private readonly Dictionary<int, ClapRoundResult> _stageResults = new();

    /// <summary>記録済みステージの有効スコア合計です。</summary>
    public int TotalScore
    {
        get
        {
            int total = 0;

            foreach (ClapRoundResult result in _stageResults.Values)
            {
                total += result.Score;
            }

            return total;
        }
    }

    /// <summary>ステージ番号をキーとしてラウンド結果を記録または更新します。</summary>
    public void Record(ClapRoundResult result)
    {
        _stageResults[result.StageNumber] = result;
    }

    /// <summary>指定したステージ番号の結果を取得します。</summary>
    public bool TryGetStageResult(int stageNumber, out ClapRoundResult result)
    {
        return _stageResults.TryGetValue(stageNumber, out result);
    }

    /// <summary>新しいゲーム開始時に、すべてのステージ結果を消去します。</summary>
    public void Reset()
    {
        _stageResults.Clear();
    }
}
