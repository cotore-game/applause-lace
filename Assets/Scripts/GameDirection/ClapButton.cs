using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

/// <summary>
/// 拍手ボタン
/// </summary>
public class ClapButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hand Sprites")]
    [SerializeField] private Image handImage;
    [SerializeField] private Sprite handOpenSprite;
    [SerializeField] private Sprite handClosedSprite;

    // イベント通知
    public event Action OnClapPressed;
    public event Action OnClapReleased;

    private RectTransform _rectTransform;
    private Vector3 _originalScale;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetHandClosed();
        ScaleDown();
        OnClapPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetHandOpen();
        ScaleUp();
        OnClapReleased?.Invoke();
    }

    private void SetHandClosed()
    {
        if (handImage != null && handClosedSprite != null)
            handImage.sprite = handClosedSprite;
    }

    private void SetHandOpen()
    {
        if (handImage != null && handOpenSprite != null)
            handImage.sprite = handOpenSprite;
    }

    private void ScaleDown()
    {
        _rectTransform.DOScale(_originalScale * 0.9f, 0.1f);
    }

    private void ScaleUp()
    {
        _rectTransform.DOScale(_originalScale, 0.1f);
    }

    public void PlayPunchAnimation()
    {
        _rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 0.5f);
    }

    public RectTransform GetRectTransform() => _rectTransform;
}
