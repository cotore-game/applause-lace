using UnityEngine;
using TMPro;
using Coffee.UIExtensions;
using GameDirection;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class GameView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ClapButton clapButton;
    [SerializeField] private CurtainController curtain;

    [Header("Effects")]
    [SerializeField] private UIParticle confettiUIParticle;
    [SerializeField] private UIParticle happyUIParticle;
    [SerializeField] private SpotlightOverlay spotlightOverlay;

    public event System.Action OnClapAction;

    private void Start()
    {
        if (clapButton != null)
        {
            clapButton.OnClapPressed += HandleClapPressed;
            clapButton.OnClapReleased += HandleClapReleased;
        }
    }

    private void OnDestroy()
    {
        if (clapButton != null)
        {
            clapButton.OnClapPressed -= HandleClapPressed;
            clapButton.OnClapReleased -= HandleClapReleased;
        }
    }

    private void HandleClapPressed()
    {
        OnClapAction?.Invoke();
    }

    private void HandleClapReleased()
    {
        // リリース時の処理
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"{score}";
    }

    public void PlayConfetti()
    {
        if (confettiUIParticle != null)
        {
            confettiUIParticle.Play();
        }

        if (clapButton != null)
        {
            clapButton.PlayPunchAnimation();
        }
    }

    public void StopClapEffects()
    {
        if (confettiUIParticle != null)
        {
            confettiUIParticle.Stop();
        }
    }

    /// <summary>
    /// ClapButtonにスポットライトを当てる（RectTransform変換対応）
    /// </summary>
    public async UniTask ShowSpotlightOnButton(float radius = 300f)
    {
        if (spotlightOverlay == null || clapButton == null) return;

        RectTransform buttonRect = clapButton.GetRectTransform();

        if (buttonRect == null)
        {
            Debug.LogError("Null buttonRect");
            return;
        }

        await spotlightOverlay.ShowAndFocusOnTarget(buttonRect, radius);
    }

    public async UniTask HideSpotlight()
    {
        if (spotlightOverlay == null) return;
        await spotlightOverlay.Hide();
    }

    public async UniTask MoveSpotlightTo(Vector2 position, float duration = 0.5f)
    {
        if (spotlightOverlay == null) return;
        await spotlightOverlay.MoveTo(position, duration);
    }

    public async UniTask MoveSpotlightToUI(RectTransform target, float duration = 0.5f)
    {
        if (spotlightOverlay == null || target == null) return;
        await spotlightOverlay.MoveToTarget(target, duration);
    }

    public void ShowResult(int finalScore)
    {
        // リザルト表示
    }

    public async UniTask ShowCurtain()
    {
        await curtain.OpenCurtainAsync();
    }

    public async UniTask HideCurtain()
    {
        await curtain.CloseCurtainAsync();
    }

    public void ChangeNextBG()
    {
        curtain.NextBackground();
    }

    /// <summary>
    /// ClapButtonを非表示
    /// </summary>
    public void HideClapButton()
    {
        if (clapButton != null)
        {
            clapButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ClapButtonをイージングインで表示
    /// </summary>
    public async UniTask ShowClapButton()
    {
        if (clapButton != null)
        {
            clapButton.gameObject.SetActive(true);
            // ボタンのスケールアニメーションなど
            var rect = clapButton.GetRectTransform();
            if (rect != null)
            {
                rect.localScale = Vector3.zero;
                await rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).ToUniTask();
            }
        }
    }

    /// <summary>
    /// スポットライトマスクを無効化（暗転なし状態）
    /// </summary>
    public void DisableSpotlightMask()
    {
        if (spotlightOverlay != null)
        {
            spotlightOverlay.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 背景をインデックスで設定
    /// </summary>
    public void SetBackground(int index)
    {
        if (curtain != null)
        {
            // CurtainControllerに背景切り替えメソッドがあれば使用
            // 例: curtain.SetBackgroundByIndex(index);
        }
    }

    /// <summary>
    /// 幕を閉じた状態にリセット
    /// </summary>
    public void ResetCurtain()
    {
        if (curtain != null)
        {
            curtain.ResetCurtainPosition();
        }
    }

    /// <summary>
    /// 幕を開けるアニメーション
    /// </summary>
    public async UniTask OpenCurtainAsync()
    {
        if (curtain != null)
        {
            await curtain.OpenCurtainAsync();
        }
    }

    /// <summary>
    /// 幕を閉じるアニメーション
    /// </summary>
    public async UniTask CloseCurtainAsync()
    {
        if (curtain != null)
        {
            await curtain.CloseCurtainAsync();
        }
    }

    /// <summary>
    /// スポットライトで画面を暗くする
    /// </summary>
    public async UniTask DimWithSpotlight()
    {
        if (spotlightOverlay != null)
        {
            spotlightOverlay.gameObject.SetActive(true);
            // 画面全体を暗くする（スポットなし）
            spotlightOverlay.ShowImmediate(Vector2.zero, 0f);
            await spotlightOverlay.SetRadius(5000f, 0.5f);
        }
    }

    /// <summary>
    /// スポットライトをクリア
    /// </summary>
    public async UniTask ClearSpotlight()
    {
        if (spotlightOverlay != null)
        {
            await spotlightOverlay.Hide();
        }
    }

    /// <summary>
    /// ハッピーエフェクトを再生
    /// </summary>
    public void PlayHappyEffect()
    {
        if (happyUIParticle != null)
        {
            happyUIParticle.Play();
        }
    }
}
