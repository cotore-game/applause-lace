using UnityEngine;
using System;
using System.Reflection;

namespace SceneManagement
{
    /// <summary>
    /// GameFlowSOに従ってシーン遷移を実行するクラス
    /// </summary>
    public class SceneFlowDirector : SingletonMonoBehaviour<SceneFlowDirector>
    {
        [SerializeField]
        private GameFlowSO _currentFlow;

        // 現在のステップインデックス（-1は開始前）
        private int _currentStepIndex = -1;

        /// <summary>
        /// 外部からフローをセットする場合（初期化）
        /// </summary>
        public void SetFlow(GameFlowSO flow)
        {
            _currentFlow = flow;
            _currentStepIndex = -1;
        }

        /// <summary>
        /// 次のシーンへの遷移を実行します。
        /// </summary>
        public void ExecuteNextTransition()
        {
            if (_currentFlow == null || _currentFlow.Steps.Count == 0)
            {
                Debug.LogWarning("[SceneFlowDirector] Flow is empty or null.");
                return;
            }

            int nextIndex = _currentStepIndex + 1;

            // リストの最後を超えた場合の処理
            if (nextIndex >= _currentFlow.Steps.Count)
            {
                if (_currentFlow.IsLoop)
                {
                    Debug.Log("[SceneFlowDirector] Loop detected. Restarting flow.");
                    nextIndex = 0;
                }
                else
                {
                    Debug.Log("[SceneFlowDirector] Reached end of flow.");
                    return; // 何もしない、あるいはタイトルに戻るなどの処理
                }
            }

            _currentStepIndex = nextIndex;
            FlowStep currentStep = _currentFlow.Steps[_currentStepIndex];

            SceneTransitioner.Instance.TransitionTo(currentStep.TargetScene, currentStep.Data);
        }
    }
}
