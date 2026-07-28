using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public sealed class StageResultView : MonoBehaviour
{
    [SerializeField] private GameObject presentationRoot;
    [SerializeField] private TextMeshProUGUI headlineText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private Animator animator;
    [SerializeField] private string showTrigger = "Show";
    [SerializeField, Min(0f)] private float displaySeconds = 2f;

    public void Prepare()
    {
        SetVisible(false);
    }

    public async UniTask ShowAsync(
        ClapRoundResult result,
        ClapStageData stageData,
        CancellationToken cancellationToken)
    {
        bool isGameOver = result.IsGameOver;

        if (headlineText != null)
        {
            headlineText.text = isGameOver
                ? stageData.GameOverHeadline
                : stageData.TimeUpHeadline;
        }

        if (scoreText != null)
        {
            scoreText.text = $"{result.Score}回";
        }

        if (commentText != null)
        {
            commentText.text = isGameOver
                ? stageData.GameOverComment
                : stageData.TimeUpComment;
        }

        SetVisible(true);

        if (animator != null && !string.IsNullOrWhiteSpace(showTrigger))
        {
            animator.ResetTrigger(showTrigger);
            animator.SetTrigger(showTrigger);
        }

        if (displaySeconds > 0f)
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(displaySeconds),
                cancellationToken: cancellationToken);
        }
    }

    private void SetVisible(bool visible)
    {
        GameObject target = presentationRoot != null ? presentationRoot : gameObject;
        target.SetActive(visible);
    }
}
