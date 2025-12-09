using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// ゲーム全体の進行フローを管理するSO
    /// </summary>
    [CreateAssetMenu(fileName = "NewGameFlow", menuName = "SceneFlow/Game Flow")]
    public class GameFlowSO : ScriptableObject
    {
        [Header("Flow Settings")]
        [Tooltip("最後のステップが終わったら最初に戻るか")]
        public bool IsLoop = false;

        [Header("Steps")]
        [SerializeField]
        private List<FlowStep> _steps = new List<FlowStep>();

        public List<FlowStep> Steps => _steps;
    }
}