using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Coffee.UISoftMask;

namespace GameDirection
{
    /// <summary>
    /// スポットライト演出
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SpotlightOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image darkOverlay; // 暗くするオーバーレイ
        [SerializeField] private RectTransform spotlightShape; // スポットライト形状

        [Header("Settings")]
        [SerializeField] private float initialRadius = 2000f; // 初期半径
        [SerializeField] private float targetRadius = 256f; // 焦点時の半径
        [SerializeField] private float fadeInDuration = 0.8f;
        [SerializeField] private float focusDuration = 0.6f; // 絞り込みアニメーション時間
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        /// <summary>
        /// スポットライトを表示（全体から徐々に絞る演出）
        /// </summary>
        /// <param name="targetPosition">焦点を当てる座標（RectTransformのローカル座標）</param>
        /// <param name="finalRadius">最終的なスポットライトの半径</param>
        public async UniTask ShowAndFocus(Vector2 targetPosition, float? finalRadius = null)
        {
            float radius = finalRadius ?? targetRadius;

            // 全画面を明るい状態からスタート
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0;

            // スポットライトは画面中央から大きくスタート
            spotlightShape.anchoredPosition = Vector2.zero;
            spotlightShape.sizeDelta = new Vector2(initialRadius * 2, initialRadius * 2);

            // オーバーレイをフェードイン
            await _canvasGroup.DOFade(1f, fadeInDuration)
                .SetEase(Ease.OutQuad)
                .AwaitForComplete();

            // スポットライトを絞り込む
            var moveTask = spotlightShape.DOAnchorPos(targetPosition, focusDuration)
                .SetEase(Ease.InOutCubic)
                .AwaitForComplete();

            var scaleTask = DOVirtual.Float(initialRadius, radius, focusDuration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutCubic)
            .AwaitForComplete();

            await UniTask.WhenAll(moveTask, scaleTask);
        }

        /// <summary>
        /// スポットライトを即座に表示（アニメーションなし）
        /// </summary>
        public void ShowImmediate(Vector2 targetPosition, float? radius = null)
        {
            float r = radius ?? targetRadius;

            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;

            spotlightShape.anchoredPosition = targetPosition;
            spotlightShape.sizeDelta = new Vector2(r * 2, r * 2);
        }

        /// <summary>
        /// スポットライトを非表示
        /// </summary>
        public async UniTask Hide(float duration = 0.3f)
        {
            await _canvasGroup.DOFade(0f, duration)
                .SetEase(Ease.InQuad)
                .AwaitForComplete();

            gameObject.SetActive(false);
        }

        /// <summary>
        /// スポットライト位置を移動
        /// </summary>
        public async UniTask MoveTo(Vector2 targetPosition, float duration = 0.5f)
        {
            await spotlightShape.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad)
                .AwaitForComplete();
        }

        /// <summary>
        /// スポットライトサイズを変更（アニメーション）
        /// </summary>
        public async UniTask SetRadius(float radius, float duration = 0.3f)
        {
            await DOVirtual.Float(spotlightShape.sizeDelta.x / 2f, radius, duration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutQuad)
            .AwaitForComplete();
        }

        /// <summary>
        /// スポットライトサイズを即座に変更
        /// </summary>
        public void SetRadiusImmediate(float radius)
        {
            spotlightShape.sizeDelta = new Vector2(radius * 2, radius * 2);
        }

        /// <summary>
        /// 位置とサイズを同時に変更
        /// </summary>
        public async UniTask MoveAndResize(Vector2 targetPosition, float radius, float duration = 0.5f)
        {
            var moveTask = spotlightShape.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad)
                .AwaitForComplete();

            var resizeTask = DOVirtual.Float(spotlightShape.sizeDelta.x / 2f, radius, duration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutQuad)
            .AwaitForComplete();

            await UniTask.WhenAll(moveTask, resizeTask);
        }
    }
}
