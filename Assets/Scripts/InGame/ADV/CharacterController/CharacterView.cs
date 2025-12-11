using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;

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
        private Canvas _rootCanvas;
        private bool _initialized;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            if (_initialized) return;

            // 初期位置をキャッシュ（イージングイン先）
            _initialPosition = rectTransform.anchoredPosition;

            // ルートCanvasを取得
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

            _initialized = true;
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
            Initialize();
            KillCurrentTween();

            // 先に表示状態にする
            characterImage.enabled = true;

            // 画面外左の位置を計算
            Vector2 offScreenLeft = CalculateOffScreenLeftPosition();

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
        /// 現在位置から画面外左へイージングアウト（その後非表示）
        /// </summary>
        public async UniTask EaseOut(float duration = 0.5f, CancellationToken cancellationToken = default)
        {
            Initialize();
            KillCurrentTween();

            // 画面外左の位置を計算
            Vector2 offScreenLeft = CalculateOffScreenLeftPosition();

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
            finally
            {
                // イージング完了後に非表示
                characterImage.enabled = false;
            }
        }

        /// <summary>
        /// 即座に非表示（画面外左へ移動 + 非表示）
        /// </summary>
        public void HideImmediate()
        {
            Initialize();
            KillCurrentTween();
            rectTransform.anchoredPosition = CalculateOffScreenLeftPosition();
            characterImage.enabled = false;
        }

        /// <summary>
        /// 即座に表示（初期位置へ移動 + 表示）
        /// </summary>
        public void ShowImmediate()
        {
            Initialize();
            KillCurrentTween();
            rectTransform.anchoredPosition = _initialPosition;
            characterImage.enabled = true;
        }

        /// <summary>
        /// 画面外左の座標を計算
        /// Anchor (0.5, 0) を考慮した計算
        /// </summary>
        private Vector2 CalculateOffScreenLeftPosition()
        {
            // CanvasのRectTransform
            RectTransform canvasRect = _rootCanvas.transform as RectTransform;

            // Canvas幅の取得
            float canvasWidth = canvasRect.rect.width;

            // 画像の幅
            float imageWidth = rectTransform.rect.width;

            // Anchor (0.5, 0) の場合、x=0 はCanvas中央, Pivotは左下
            float offScreenX = - (imageWidth / 2f);

            // Y座標は初期位置を維持
            return new Vector2(offScreenX, _initialPosition.y);
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
