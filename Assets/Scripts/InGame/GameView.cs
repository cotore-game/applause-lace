using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Coffee.UIExtensions;
using GameDirection;
using Cysharp.Threading.Tasks;

public class GameView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ClapButton clapButton;

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

    public void SetCharacter(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Claps: {score}";
        UpdateCharacterExpression(score);
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

    /// <summary>
    /// スポットライトを表示してキャラクターに焦点を当てる
    /// </summary>
    public async UniTask ShowSpotlightOnCharacter(float radius = 256f)
    {
        if (spotlightOverlay == null) return;
        await spotlightOverlay.ShowAndFocusOnTarget(characterImage.rectTransform, radius);
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

    private void UpdateCharacterExpression(int score)
    {
        // キャラクター表情変化ロジック
    }
}
