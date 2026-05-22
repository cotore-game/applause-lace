using UnityEngine;

[CreateAssetMenu(fileName = "ClapStageData", menuName = "Game/ClapStageData")]
public class ClapStageData : ScriptableObject
{
    public int stageNumber;

    [Header("チキンレース時間設定")]
    public float baseTime = 5.0f; // 基本となる演説時間
    public float randomRange = 0.5f; // +-のブレ幅（0.5なら baseTime +-0.5秒）
    public float limitOffset = 0.5f; // ターゲット時間からアウトまでの猶予時間
}
