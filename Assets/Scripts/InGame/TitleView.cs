using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class TitleView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image titleLogo;
    [SerializeField] private TMP_Text pressAnyKeyText;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// タイトル画面を表示
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);

        // Press Any Key の点滅アニメーション
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.DOFade(0.3f, 0.8f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    /// <summary>
    /// キー入力待機
    /// </summary>
    public async UniTask WaitForAnyKey()
    {
        await UniTask.WaitUntil(() => Input.anyKeyDown);
    }

    /// <summary>
    /// タイトル画面を非表示
    /// </summary>
    public async UniTask Hide()
    {
        // 点滅アニメーションを停止
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.DOKill();
        }

        await canvasGroup.DOFade(0f, 0.5f).ToUniTask();
        gameObject.SetActive(false);
    }
}
