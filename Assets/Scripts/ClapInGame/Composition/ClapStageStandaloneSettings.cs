using System;
using SceneManagement;

/// <summary>
/// ステージシーンをGameFlow外から直接再生するときだけ使う開発用設定です。
/// GameFlow管理下では<see cref="ClapStageRunContext"/>が常に優先されます。
/// </summary>
public sealed class ClapStageStandaloneSettings
{
    /// <summary>直接再生時に使用する論理シーンIDです。</summary>
    public SceneId SceneId { get; }

    /// <summary>直接再生時に使用するステージデータです。</summary>
    public ClapStageData StageData { get; }

    /// <summary>直接再生用の設定を生成します。</summary>
    public ClapStageStandaloneSettings(SceneId sceneId, ClapStageData stageData)
    {
        SceneId = sceneId;
        StageData = stageData != null
            ? stageData
            : throw new ArgumentNullException(nameof(stageData));
    }
}
