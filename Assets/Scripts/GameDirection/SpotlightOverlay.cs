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
        [SerializeField] private Image darkOverlay;
        [SerializeField] private RectTransform spotlightShape;
        [SerializeField] private SoftMask softMask;
        [SerializeField] private Canvas targetCanvas; // Canvas参照

        [Header("Settings")]
        [SerializeField] private float initialRadius = 2000f;
        [SerializeField] private float targetRadius = 256f;
        [SerializeField] private float fadeInDuration = 0.8f;
        [SerializeField] private float focusDuration = 0.6f;

        private CanvasGroup _canvasGroup;
        private RectTransform _rootRectTransform;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _rootRectTransform = GetComponent<RectTransform>();

            // Canvasが未設定なら自動検索
            if (targetCanvas == null)
                targetCanvas = GetComponentInParent<Canvas>();

            if (_rootRectTransform == null)
                Debug.LogError("[SpotlightOverlay] RectTransform not found!");
            if (targetCanvas == null)
                Debug.LogError("[SpotlightOverlay] Parent Canvas not found!");

            gameObject.SetActive(false);
        }

        /// <summary>
        /// RectTransformをターゲットにスポットライトを表示
        /// </summary>
        public async UniTask ShowAndFocusOnTarget(RectTransform target, float? finalRadius = null)
        {
            if (target == null) return;

            // ターゲットのローカル座標を取得
            Vector2 localPosition = ConvertToLocalPosition(target);
            await ShowAndFocus(localPosition, finalRadius);
        }

        /// <summary>
        /// スポットライトを表示（全体から徐々に絞る演出）
        /// </summary>
        public async UniTask ShowAndFocus(Vector2 targetPosition, float? finalRadius = null)
        {
            float radius = finalRadius ?? targetRadius;

            gameObject.SetActive(true);
            _canvasGroup.alpha = 0;

            // 画面中央から大きくスタート
            spotlightShape.anchoredPosition = Vector2.zero;
            spotlightShape.sizeDelta = new Vector2(initialRadius * 2, initialRadius * 2);

            // オーバーレイをフェードイン
            var tween1 = _canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
            await UniTask.WaitUntil(() => !tween1.IsActive());

            // スポットライトを絞り込む
            var tween2 = spotlightShape.DOAnchorPos(targetPosition, focusDuration)
                .SetEase(Ease.InOutCubic);

            var tween3 = DOVirtual.Float(initialRadius, radius, focusDuration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutCubic);

            await UniTask.WaitUntil(() => !tween2.IsActive() && !tween3.IsActive());
        }

        public void ShowImmediate(Vector2 targetPosition, float? radius = null)
        {
            float r = radius ?? targetRadius;

            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;

            spotlightShape.anchoredPosition = targetPosition;
            spotlightShape.sizeDelta = new Vector2(r * 2, r * 2);
        }

        public async UniTask Hide(float duration = 0.3f)
        {
            var tween = _canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
            await UniTask.WaitUntil(() => !tween.IsActive());
            gameObject.SetActive(false);
        }

        public async UniTask MoveTo(Vector2 targetPosition, float duration = 0.5f)
        {
            var tween = spotlightShape.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad);
            await UniTask.WaitUntil(() => !tween.IsActive());
        }

        public async UniTask MoveToTarget(RectTransform target, float duration = 0.5f)
        {
            if (target == null) return;
            Vector2 localPosition = ConvertToLocalPosition(target);
            await MoveTo(localPosition, duration);
        }

        public async UniTask SetRadius(float radius, float duration = 0.3f)
        {
            var tween = DOVirtual.Float(spotlightShape.sizeDelta.x / 2f, radius, duration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutQuad);

            await UniTask.WaitUntil(() => !tween.IsActive());
        }

        public void SetRadiusImmediate(float radius)
        {
            spotlightShape.sizeDelta = new Vector2(radius * 2, radius * 2);
        }

        public async UniTask MoveAndResize(Vector2 targetPosition, float radius, float duration = 0.5f)
        {
            var tween1 = spotlightShape.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad);

            var tween2 = DOVirtual.Float(spotlightShape.sizeDelta.x / 2f, radius, duration, value =>
            {
                spotlightShape.sizeDelta = new Vector2(value * 2, value * 2);
            }).SetEase(Ease.InOutQuad);

            await UniTask.WaitUntil(() => !tween1.IsActive() && !tween2.IsActive());
        }

        /// <summary>
        /// ターゲットのRectTransformをこのSpotlightOverlayのローカル座標に変換
        /// </summary>
        private Vector2 ConvertToLocalPosition(RectTransform target)
        {
            // ターゲットのワールド座標を取得
            Vector3 worldPos = target.position;

            // Overlayが属するCanvasのカメラを取得
            Camera cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;

            // ワールド座標 -> スクリーン座標
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

            // スクリーン座標 -> SpotlightOverlay内のローカル座標
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootRectTransform,
                screenPoint,
                cam,
                out Vector2 localPoint
            );

            return localPoint;
        }
    }
}
