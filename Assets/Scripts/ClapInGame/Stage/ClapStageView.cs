using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ClapStageView : MonoBehaviour, IClapStageSceneView
{
    [Header("ゲーム中Prefab")]
    [SerializeField] private ClapHandButtonView clapHandButton;
    [SerializeField] private CountPanelView countPanel;
    [SerializeField] private CountdownView countdown;
    [SerializeField] private ClapConfettiView confetti;

    [Header("ADV Prefab")]
    [SerializeField] private DialogueView dialogue;
    [SerializeField] private StageDialogueEventView dialogueEvents;

    [Header("ステージ演出Prefab")]
    [SerializeField] private HostCutInView hostCutInStart;
    [SerializeField] private HostCutInView hostCutInFinish;
    [SerializeField] private StageResultView result;

    public CountdownView Countdown => countdown;
    public DialogueView Dialogue => dialogue;
    public StageDialogueEventView DialogueEvents => dialogueEvents;

    public event Action OnClapButtonClicked;

    private void Awake()
    {
        if (clapHandButton != null)
        {
            clapHandButton.OnClapCompleted += HandleClapCompleted;
        }
    }

    private void OnDestroy()
    {
        if (clapHandButton != null)
        {
            clapHandButton.OnClapCompleted -= HandleClapCompleted;
        }
    }

    public void PrepareStage()
    {
        UpdateClapCount(0);
        SetClapInputEnabled(false);
        clapHandButton?.SetVisible(false);
        countPanel?.SetVisible(false);
        countdown?.Prepare();
        confetti?.Prepare();
        dialogue?.Hide();
        hostCutInStart?.Prepare();
        hostCutInFinish?.Prepare();
        result?.Prepare();
    }

    public void BeginGameplay()
    {
        UpdateClapCount(0);
        countPanel?.SetVisible(true);
        clapHandButton?.SetVisible(true);
        SetClapInputEnabled(true);
    }

    public void EndGameplay()
    {
        SetClapInputEnabled(false);
        clapHandButton?.SetVisible(false);
        countPanel?.SetVisible(false);
    }

    public void UpdateClapCount(int count)
    {
        countPanel?.SetCount(count);
    }

    /// <summary>成立した拍手に対応する視覚効果を1回再生します。</summary>
    public void PlayClapEffect()
    {
        confetti?.PlayBurst();
    }

    public void SetClapInputEnabled(bool enabled)
    {
        clapHandButton?.SetInputEnabled(enabled);
    }

    public void ShowStartCutIn()
    {
        hostCutInStart?.Show();
    }

    public void HideStartCutIn()
    {
        hostCutInStart?.Hide();
    }

    public UniTask PlayFinishCutInAsync(CancellationToken cancellationToken)
    {
        return hostCutInFinish != null
            ? hostCutInFinish.PlayAsync(cancellationToken)
            : UniTask.CompletedTask;
    }

    public UniTask ShowResultAsync(
        ClapRoundResult roundResult,
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        return result != null
            ? result.ShowAsync(roundResult, stageData, cancellationToken)
            : UniTask.CompletedTask;
    }

    private void HandleClapCompleted()
    {
        OnClapButtonClicked?.Invoke();
    }
}
