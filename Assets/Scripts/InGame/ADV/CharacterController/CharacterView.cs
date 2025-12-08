using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

namespace ADV.Presentation
{
    /// <summary>
    /// 個別キャラクターのView
    /// </summary>
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private CanvasGroup canvasGroup;

        private Tween _currentTween;

        public void SetSprite(Sprite sprite)
        {
            Debug.Log(sprite);
            characterImage.sprite = sprite;
        }

        public void SetAlpha(float alpha)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public async UniTask FadeIn(float duration, CancellationToken cancellationToken = default)
        {
            KillCurrentTween();

            if (canvasGroup == null) return;

            _currentTween = canvasGroup.DOFade(1f, duration)
                .SetEase(Ease.OutQuad);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                SetAlpha(1f);
            }
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        public async UniTask FadeOut(float duration, CancellationToken cancellationToken = default)
        {
            KillCurrentTween();

            if (canvasGroup == null) return;

            _currentTween = canvasGroup.DOFade(0f, duration)
                .SetEase(Ease.OutQuad);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                SetAlpha(0f);
            }
        }

        /// <summary>
        /// 指定座標へ移動
        /// </summary>
        public async UniTask MoveTo(Vector3 targetPosition, float duration, CancellationToken cancellationToken = default)
        {
            KillCurrentTween();

            _currentTween = transform.DOLocalMove(targetPosition, duration)
                .SetEase(Ease.InOutQuad);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                transform.localPosition = targetPosition;
            }
        }

        /// <summary>
        /// 現在実行中のTweenを停止
        /// </summary>
        private void KillCurrentTween()
        {
            if (_currentTween != null && _currentTween.IsActive())
            {
                _currentTween.Kill();
                _currentTween = null;
            }
        }

        private void OnDestroy()
        {
            KillCurrentTween();
        }
    }
}
