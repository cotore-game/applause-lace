using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// シーン遷移データの基底クラス
    /// </summary>
    public abstract class ScriptableSceneData : ScriptableObject, ISceneExchangeData
    {
        [SerializeField, Tooltip("このデータを使用する対象のシーンID")]
        private SceneId _targetSceneId;

        public SceneId TargetSceneId => _targetSceneId;
    }
}