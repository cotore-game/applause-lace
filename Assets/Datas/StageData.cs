using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData")]
public class StageData : ScriptableObject
{
    [Header("ステージ名 (\"卒業式\"など)")]
    public string StageName;

    [Header("キャラ画像")]
    public Sprite CharacterImage;

    [Header("制限時間")]
    public float Duration;

    [Header("フィーバー発生確率"), Range(0,1)]
    public float FeverProbability;

    [Header("チュートリアルフラグ")]
    public bool IsTutorial;

    // 会話データなどは別途CSVパースからの、クラス化して持たせようかなという予定
}
