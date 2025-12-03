using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 拍手ボタン
/// </summary>
public class ClapButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hand Sprites")]
    [SerializeField] private Image handImage;
    [SerializeField] private Sprite handOpenSprite;
    [SerializeField] private Sprite handClosedSprite;

    [Header("Effects")]
    [SerializeField] private ParticleSystem confettiParticle;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 手を閉じる
        handImage.sprite = handClosedSprite;

        // ボタンを少し押し込む演出
        transform.DOScale(0.9f, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 手を開く
        handImage.sprite = handOpenSprite;

        // ボタンを元に戻す
        transform.DOScale(1f, 0.1f);

        // エフェクト再生
        PlayEffects();
    }

    private void PlayEffects()
    {
        // 紙吹雪
        if (confettiParticle != null)
        {
            confettiParticle.Play();
        }

        // ボタンにパンチ演出
        transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 0.5f);
    }
}
