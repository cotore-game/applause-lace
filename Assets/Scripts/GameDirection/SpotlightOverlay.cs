using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace GameDirection
{
    /// <summary>
    /// スポットライト演出
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class SpotlightOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image overlayImage; // 暗くするオーバーレイ
        [SerializeField] private Image spotlightMask; // 光る部分のマスク
        [SerializeField] private RectTransform spotlightTransform;

        [Header("Settings")]
        [SerializeField] private Color darkColor = new Color(0, 0, 0, 0.8f);
        [SerializeField] private float spotlightRadius = 200f;
        [SerializeField] private float fadeInDuration = 0.5f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        /// <summary>
        /// スポットライトを表示
        /// </summary>
        public async UniTask Show(Vector2 targetPosition)
        {
            // 初期設定
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0;
            spotlightTransform.anchoredPosition = targetPosition;
            spotlightTransform.sizeDelta = new Vector2(spotlightRadius * 2, spotlightRadius * 2);

            // フェードイン
            await _canvasGroup.DOFade(1f, fadeInDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// スポットライトを非表示
        /// </summary>
        public async UniTask Hide()
        {
            await _canvasGroup.DOFade(0f, 0.3f)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        /// <summary>
        /// スポットライト位置を移動
        /// </summary>
        public async UniTask MoveTo(Vector2 targetPosition, float duration = 0.5f)
        {
            await spotlightTransform.DOAnchorPos(targetPosition, duration)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// スポットライトサイズを変更
        /// </summary>
        public void SetRadius(float radius)
        {
            spotlightRadius = radius;
            spotlightTransform.sizeDelta = new Vector2(radius * 2, radius * 2);
        }
    }
}
