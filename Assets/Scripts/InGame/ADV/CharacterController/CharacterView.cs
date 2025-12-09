using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

namespace ADV.Presentation
{
    /// <summary>
    /// 単体キャラクター立ち絵のView
    /// 画面外左からイージングイン/アウトに対応
    /// </summary>
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private RectTransform rectTransform;

        private Vector2 _initialPosition;
        private Tween _currentTween;

        private void Awake()
        {
            // 初期位置をキャッシュ（イージングイン先）
            _initialPosition = rectTransform.anchoredPosition;
        }

        /// <summary>
        /// スプライトを設定（即座に切り替え）
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            if (characterImage != null)
            {
                characterImage.sprite = sprite;
            }
        }

        /// <summary>
        /// 画面外左から初期位置へイージングイン
        /// </summary>
        public async UniTask EaseIn(float duration = 0.5f, CancellationToken cancellationToken = default)
        {
            KillCurrentTween();

            // 画面外左の位置を計算（画面幅の左端より外）
            var canvas = GetComponentInParent<Canvas>();
            var canvasRect = (canvas.transform as RectTransform).rect;
            float screenLeftEdge = -canvasRect.width / 2f;

            // さらに画像幅分左にオフセット
            float imageWidth = rectTransform.rect.width;
            Vector2 offScreenLeft = new Vector2(screenLeftEdge - imageWidth, _initialPosition.y);

            // 開始位置を画面外左に設定
            rectTransform.anchoredPosition = offScreenLeft;

            // 初期位置へイージング
            _currentTween = rectTransform.DOAnchorPos(_initialPosition, duration)
                .SetEase(Ease.OutCubic);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                rectTransform.anchoredPosition = _initialPosition;
            }
        }

        /// <summary>
        /// 現在位置から画面外左へイージングアウト
        /// </summary>
        public async UniTask EaseOut(float duration = 0.5f, CancellationToken cancellationToken = default)
        {
            KillCurrentTween();

            // 画面外左の位置を計算
            var canvas = GetComponentInParent<Canvas>();
            var canvasRect = (canvas.transform as RectTransform).rect;
            float screenLeftEdge = -canvasRect.width / 2f;

            float imageWidth = rectTransform.rect.width;
            Vector2 offScreenLeft = new Vector2(screenLeftEdge - imageWidth, rectTransform.anchoredPosition.y);

            // 画面外左へイージング
            _currentTween = rectTransform.DOAnchorPos(offScreenLeft, duration)
                .SetEase(Ease.InCubic);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                rectTransform.anchoredPosition = offScreenLeft;
            }
        }

        /// <summary>
        /// 即座に非表示（画面外左へ移動）
        /// </summary>
        public void HideImmediate()
        {
            KillCurrentTween();

            var canvas = GetComponentInParent<Canvas>();
            var canvasRect = (canvas.transform as RectTransform).rect;
            float screenLeftEdge = -canvasRect.width / 2f;
            float imageWidth = rectTransform.rect.width;

            rectTransform.anchoredPosition = new Vector2(screenLeftEdge - imageWidth, _initialPosition.y);
        }

        /// <summary>
        /// 即座に表示（初期位置へ移動）
        /// </summary>
        public void ShowImmediate()
        {
            KillCurrentTween();
            rectTransform.anchoredPosition = _initialPosition;
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
