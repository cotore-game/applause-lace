using System;
using System.Collections.Generic;
using SceneManagement;
using UnityEngine;

/// <summary>
/// ゲーム全体で巡回するコンテンツシーンの順序と、遷移演出設定を定義します。
/// </summary>
[CreateAssetMenu(fileName = "GameFlowDefinition", menuName = "Game/Game Flow Definition")]
public sealed class GameFlowDefinition : ScriptableObject
{
    /// <summary>1つのコンテンツシーンを実行するための設定です。</summary>
    [Serializable]
    public sealed class Step
    {
        [SerializeField] private SceneId sceneId;
        [SerializeField] private ClapStageData stageData;
        [SerializeField] private bool openCurtainBeforeEnter = true;
        [SerializeField] private bool closeCurtainOnComplete = true;

        /// <summary>ロードするコンテンツシーンの識別子です。</summary>
        public SceneId SceneId => sceneId;

        /// <summary>ステージ看板に使うデータです。ステージ以外ではnullにします。</summary>
        public ClapStageData StageData => stageData;

        /// <summary>コンテンツ開始許可の前に幕を開けるかを示します。</summary>
        public bool OpenCurtainBeforeEnter => openCurtainBeforeEnter;

        /// <summary>コンテンツ完了後、アンロード前に幕を閉じるかを示します。</summary>
        public bool CloseCurtainOnComplete => closeCurtainOnComplete;
    }

    [SerializeField] private List<Step> steps = new();
    [SerializeField] private bool loop = true;

    /// <summary>実行順に並んだシーン設定です。</summary>
    public IReadOnlyList<Step> Steps => steps;

    /// <summary>末尾まで実行した後、先頭から再開するかを示します。</summary>
    public bool Loop => loop;
}
