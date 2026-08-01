using System;
using SceneManagement;

/// <summary>
/// GameCoreが選択した、1回の拍手ステージ実行に必要な不変の設定です。
/// </summary>
public sealed class ClapStageRunContext
{
    /// <summary>実行対象の論理シーンIDです。</summary>
    public SceneId SceneId { get; }

    /// <summary>看板、ゲームバランス、会話、リザルトに共通して使うステージデータです。</summary>
    public ClapStageData StageData { get; }

    /// <summary>実行対象とステージデータを組み合わせたコンテキストを生成します。</summary>
    public ClapStageRunContext(SceneId sceneId, ClapStageData stageData)
    {
        SceneId = sceneId;
        StageData = stageData != null
            ? stageData
            : throw new ArgumentNullException(nameof(stageData));
    }
}

/// <summary>
/// Additiveロードされたステージへ、GameCoreが選択した実行設定を公開します。
/// </summary>
public interface IClapStageRunContextProvider
{
    /// <summary>GameFlowが現在実行している拍手ステージの設定を取得します。</summary>
    bool TryGetCurrent(out ClapStageRunContext context);
}

/// <summary>
/// GameFlowの実行中だけ拍手ステージ設定を保持します。
/// ライフサイクルの開始・終了は<see cref="GameFlowController"/>が所有します。
/// </summary>
public sealed class ClapStageRunContextStore : IClapStageRunContextProvider
{
    private ClapStageRunContext _current;

    /// <inheritdoc />
    public bool TryGetCurrent(out ClapStageRunContext context)
    {
        context = _current;
        return context != null;
    }

    internal void Begin(ClapStageRunContext context)
    {
        if (_current != null)
        {
            throw new InvalidOperationException(
                $"拍手ステージはすでに実行中です: {_current.SceneId}");
        }

        _current = context ?? throw new ArgumentNullException(nameof(context));
    }

    internal void End(ClapStageRunContext context)
    {
        if (ReferenceEquals(_current, context))
        {
            _current = null;
        }
    }
}
