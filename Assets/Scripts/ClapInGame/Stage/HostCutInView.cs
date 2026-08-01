using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class HostCutInView : MonoBehaviour
{
    [Header("共通")]
    [SerializeField] private Animator animator;
    [SerializeField] private string showTrigger = "Show";

    [Header("終了カットイン横断アニメーション")]
    [SerializeField] private RectTransform visualRect;
    [SerializeField, Min(0f)] private float enterDuration = 0.5f;
    [SerializeField] private Ease enterEase = Ease.InCubic;
    [SerializeField, Min(0f)] private float centerHoldSeconds = 1f;
    [SerializeField, Min(0f)] private float exitDuration = 0.5f;
    [SerializeField] private Ease exitEase = Ease.OutCubic;
    [SerializeField, Min(0f)] private float offscreenPadding;

    private RectTransform _presentationRect;
    private Vector2 _centerAnchoredPosition;
    private bool _hasCenterPosition;

    public void Prepare()
    {
        CacheRectTransform();
        _presentationRect?.DOKill();

        if (_presentationRect != null && _hasCenterPosition)
        {
            _presentationRect.anchoredPosition = _centerAnchoredPosition;
        }

        gameObject.SetActive(false);
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken)
    {
        CacheRectTransform();

        if (_presentationRect == null
            || _presentationRect.parent is not RectTransform viewport)
        {
            Debug.LogWarning($"{name}: 終了カットインのRectTransformまたは親RectTransformがありません。");
            return;
        }

        _presentationRect.DOKill();
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();

        _presentationRect.anchoredPosition = _centerAnchoredPosition;
        RectTransform boundsTarget = visualRect != null
            ? visualRect
            : _presentationRect;
        Bounds centeredBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(
            viewport,
            boundsTarget);

        float startOffset = viewport.rect.xMax
            + offscreenPadding
            - centeredBounds.min.x;
        float exitOffset = viewport.rect.xMin
            - offscreenPadding
            - centeredBounds.max.x;

        float centerX = _centerAnchoredPosition.x;
        float startX = centerX + startOffset;
        float exitX = centerX + exitOffset;

        _presentationRect.anchoredPosition = new Vector2(
            startX,
            _centerAnchoredPosition.y);

        Sequence sequence = DOTween.Sequence();
        _ = sequence.Append(
            _presentationRect
                .DOAnchorPosX(centerX, enterDuration)
                .SetEase(enterEase));
        _ = sequence.AppendInterval(centerHoldSeconds);
        _ = sequence.Append(
            _presentationRect
                .DOAnchorPosX(exitX, exitDuration)
                .SetEase(exitEase));

        try
        {
            await sequence.ToUniTask(
                TweenCancelBehaviour.KillAndCancelAwait,
                cancellationToken);
        }
        finally
        {
            _presentationRect.DOKill();
            _presentationRect.anchoredPosition = _centerAnchoredPosition;
            gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        CacheRectTransform();

        if (_presentationRect != null)
        {
            _presentationRect.DOKill();
            _presentationRect.anchoredPosition = _centerAnchoredPosition;
        }

        gameObject.SetActive(true);

        if (animator != null && !string.IsNullOrWhiteSpace(showTrigger))
        {
            animator.ResetTrigger(showTrigger);
            animator.SetTrigger(showTrigger);
        }
    }

    public void Hide()
    {
        CacheRectTransform();
        _presentationRect?.DOKill();

        if (_presentationRect != null)
        {
            _presentationRect.anchoredPosition = _centerAnchoredPosition;
        }

        gameObject.SetActive(false);
    }

    private void CacheRectTransform()
    {
        if (_presentationRect == null)
        {
            _presentationRect = transform as RectTransform;
        }

        if (visualRect == null)
        {
            Image visualImage = GetComponentInChildren<Image>(true);
            visualRect = visualImage != null
                ? visualImage.rectTransform
                : _presentationRect;
        }

        if (_presentationRect != null && !_hasCenterPosition)
        {
            _centerAnchoredPosition = _presentationRect.anchoredPosition;
            _hasCenterPosition = true;
        }
    }

    private void OnDestroy()
    {
        _presentationRect?.DOKill();
    }
}
