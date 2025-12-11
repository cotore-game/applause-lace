using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class CountdownView : MonoBehaviour
{
    [Header("Countdown Images")]
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite[] countdownSprites; // 3, 2, 1, Start の画像

    [Header("UI Elements")]
    [SerializeField] private TMP_Text countText;
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
    /// カウントをリセット
    /// </summary>
    public void ResetCount()
    {
        if (countText != null)
        {
            countText.text = "0";
        }
    }

    /// <summary>
    /// カウントダウンUIを表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// カウントダウンアニメーションを再生 (3, 2, 1, Start!)
    /// </summary>
    public async UniTask PlayCountdown()
    {
        if (countdownSprites == null || countdownSprites.Length < 4)
        {
            Debug.LogWarning("Countdown sprites not set properly");
            return;
        }

        for (int i = 0; i < countdownSprites.Length; i++)
        {
            if (countdownImage != null)
            {
                countdownImage.sprite = countdownSprites[i];
                countdownImage.transform.localScale = Vector3.zero;

                // スケールアニメーション
                await countdownImage.transform
                    .DOScale(Vector3.one, 0.3f)
                    .SetEase(Ease.OutBack)
                    .ToUniTask();

                await UniTask.Delay(700);

                // フェードアウト
                await countdownImage
                    .DOFade(0f, 0.2f)
                    .ToUniTask();

                countdownImage.color = Color.white; // リセット
            }
        }

        // カウントダウン終了後、非表示にする
        gameObject.SetActive(false);
    }

    /// <summary>
    /// カウントダウンUIを非表示
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}