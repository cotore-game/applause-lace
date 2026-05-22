using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ResultView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// リザルトを表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }

    /// <summary>
    /// スコアとメッセージを設定
    /// </summary>
    public void ShowResult(int score, string message)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    /// <summary>
    /// リザルトを非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
