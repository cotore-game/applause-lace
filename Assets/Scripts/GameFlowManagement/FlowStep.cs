using System;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// 1つの遷移ステップ定義
    /// </summary>
    [Serializable]
    public class FlowStep
    {
        [Tooltip("エディタでの識別用")]
        public string Description;

        [Tooltip("遷移先のシーンID")]
        public SceneId TargetScene;

        [Tooltip("渡すデータ (任意) / Noneの場合はデータなし遷移になります。")]
        public ScriptableSceneData Data;
    }
}
