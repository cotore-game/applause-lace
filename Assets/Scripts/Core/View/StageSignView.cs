using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class StageSignView : MonoBehaviour
{
    private const float MinimumAllowedDisplaySeconds = 2f;

    [Header("看板の参照")]
    [SerializeField] private RectTransform stageSignRect;
    [SerializeField] private Image stageSignImage;

    [Header("移動先")]
    [SerializeField] private RectTransform initPosition;
    [SerializeField] private RectTransform shownPosition;

    [Header("登場アニメーション")]
    [SerializeField, Min(0f)] private float enterDuration = 0.5f;
    [SerializeField] private Ease enterEase = Ease.OutBack;
    [SerializeField, Min(MinimumAllowedDisplaySeconds)]
    private float minimumDisplaySeconds = MinimumAllowedDisplaySeconds;

    [Header("退場アニメーション")]
    [SerializeField, Min(0f)] private float exitDuration = 0.5f;
    [SerializeField] private Ease exitEase = Ease.InBack;

    private bool _isPlaying;

    public void Prepare(Sprite stageSignSprite)
    {
        stageSignRect?.DOKill();
        _isPlaying = false;

        if (stageSignImage != null)
        {
            stageSignImage.sprite = stageSignSprite;
            stageSignImage.enabled = stageSignSprite != null;
        }

        if (stageSignRect != null && initPosition != null)
        {
            stageSignRect.localPosition = initPosition.localPosition;
        }

        SetVisible(false);
    }

    public async UniTask EnterAsync(
        Sprite stageSignSprite,
        CancellationToken cancellationToken)
    {
        if (!HasRequiredReferences())
        {
            return;
        }

        if (stageSignSprite == null)
        {
            Debug.LogWarning($"{name}: ステージ看板のSpriteが設定されていません。");
            Prepare(null);
            return;
        }

        if (_isPlaying)
        {
            Debug.LogWarning($"{name}: ステージ看板の登場演出が重複して呼び出されました。");
            return;
        }

        Prepare(stageSignSprite);
        cancellationToken.ThrowIfCancellationRequested();

        _isPlaying = true;
        SetVisible(true);

        try
        {
            UniTask minimumDisplayTask = UniTask.Delay(
                TimeSpan.FromSeconds(
                    Mathf.Max(MinimumAllowedDisplaySeconds, minimumDisplaySeconds)),
                cancellationToken: cancellationToken);

            if (enterDuration <= 0f)
            {
                stageSignRect.localPosition = shownPosition.localPosition;
                await minimumDisplayTask;
                return;
            }

            UniTask enterTask = stageSignRect
                .DOLocalMove(shownPosition.localPosition, enterDuration)
                .SetEase(enterEase)
                .ToUniTask(
                    TweenCancelBehaviour.KillAndCancelAwait,
                    cancellationToken);

            await UniTask.WhenAll(enterTask, minimumDisplayTask);
        }
        finally
        {
            _isPlaying = false;
        }
    }

    public async UniTask ExitAsync(CancellationToken cancellationToken)
    {
        if (!HasRequiredReferences())
        {
            return;
        }

        if (!stageSignRect.gameObject.activeSelf)
        {
            return;
        }

        if (_isPlaying)
        {
            Debug.LogWarning($"{name}: ステージ看板の演出が重複して呼び出されました。");
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        _isPlaying = true;

        try
        {
            if (exitDuration <= 0f)
            {
                stageSignRect.localPosition = initPosition.localPosition;
                return;
            }

            await stageSignRect
                .DOLocalMove(initPosition.localPosition, exitDuration)
                .SetEase(exitEase)
                .ToUniTask(
                    TweenCancelBehaviour.KillAndCancelAwait,
                    cancellationToken);
        }
        finally
        {
            _isPlaying = false;
            SetVisible(false);
        }
    }

    public void Hide()
    {
        stageSignRect?.DOKill();
        _isPlaying = false;
        SetVisible(false);
    }

    private bool HasRequiredReferences()
    {
        if (stageSignRect == null
            || stageSignImage == null
            || initPosition == null
            || shownPosition == null)
        {
            Debug.LogWarning($"{name}: StageSignViewの参照が設定されていません。");
            return false;
        }

        Transform expectedParent = stageSignRect.parent;

        if (initPosition.parent != expectedParent || shownPosition.parent != expectedParent)
        {
            Debug.LogWarning(
                $"{name}: 看板本体とPositionオブジェクトは同じ親の子に配置してください。");
            return false;
        }

        return true;
    }

    private void SetVisible(bool visible)
    {
        if (stageSignRect != null)
        {
            stageSignRect.gameObject.SetActive(visible);
        }
    }

    private void OnDestroy()
    {
        stageSignRect?.DOKill();
    }
}
