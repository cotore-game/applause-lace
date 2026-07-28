using UnityEngine;

[CreateAssetMenu(fileName = "ClapStageData", menuName = "Game/ClapStageData")]
public class ClapStageData : ScriptableObject
{
    public int stageNumber;

    [Header("チキンレース時間設定")]
    public float baseTime = 5.0f; // 拍手してよい基本時間
    public float randomRange = 0.5f; // 拍手期限の±ブレ幅
    public float limitOffset = 0.5f; // 拍手期限からラウンド終了までの時間
}
