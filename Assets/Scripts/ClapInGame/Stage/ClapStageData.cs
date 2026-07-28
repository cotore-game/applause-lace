using UnityEngine;

[CreateAssetMenu(fileName = "ClapStageData", menuName = "Game/ClapStageData")]
public sealed class ClapStageData : ScriptableObject
{
    [Header("ステージ情報")]
    [SerializeField, Min(1)] private int stageNumber = 1;
    [SerializeField] private string stageTitle;

    [Header("チキンレース時間設定")]
    [SerializeField, Min(0f)] private float baseTime = 5.0f;
    [SerializeField, Min(0f)] private float randomRange = 0.5f;
    [SerializeField, Min(0f)] private float limitOffset = 0.5f;

    [Header("セリフパート")]
    [SerializeField] private StageDialogueData dialogue;

    [Header("リザルト文言")]
    [SerializeField] private string timeUpHeadline = "TIME UP!";
    [SerializeField, TextArea] private string timeUpComment;
    [SerializeField] private string gameOverHeadline = "GAME OVER";
    [SerializeField, TextArea] private string gameOverComment;

    public int StageNumber => stageNumber;
    public string StageTitle => stageTitle;

    public float BaseTime => baseTime;
    public float RandomRange => randomRange;
    public float LimitOffset => limitOffset;

    public StageDialogueData Dialogue => dialogue;

    public string TimeUpHeadline => timeUpHeadline;
    public string TimeUpComment => timeUpComment;
    public string GameOverHeadline => gameOverHeadline;
    public string GameOverComment => gameOverComment;

    private void OnValidate()
    {
        randomRange = Mathf.Min(randomRange, baseTime);
    }
}
