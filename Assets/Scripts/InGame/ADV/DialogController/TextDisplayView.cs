using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace ADV.Presentation
{
    /// <summary>
    /// テキスト表示のView
    /// 画面外右からイージングイン/アウトに対応
    /// </summary>
    public class TextDisplayView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private GameObject skipIcon;

        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;

        private RectTransform _speechBubbleRect;
        private Vector2 _initialPosition; // エディタで設定された初期位置（変更されない）
        private Canvas _rootCanvas;
        private Tween _currentTween;
        private bool _initialized;

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化（一度だけ実行）
        /// </summary>
        private void Initialize()
        {
            if (_initialized) return;

            // speechBubbleのRectTransformを取得（実際に動かす対象）
            _speechBubbleRect = speechBubble.GetComponent<RectTransform>();
            if (_speechBubbleRect == null)
            {
                Debug.LogError("[TextDisplayView] speechBubble must have RectTransform");
                return;
            }

            // エディタで設定された初期位置を保存（この値は二度と変更しない）
            _initialPosition = _speechBubbleRect.anchoredPosition;

            // ルートCanvasを取得
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

            _initialized = true;

            Debug.Log($"[TextDisplayView] Initialized with initial position: {_initialPosition}");
        }

        /// <summary>
        /// 画面外右から初期位置へイージングイン（表示）
        /// </summary>
        public async UniTask EaseInFromOffScreen(CancellationToken cancellationToken = default)
        {
            if (!_initialized)
            {
                Debug.LogError("[TextDisplayView] Not initialized! Call Initialize() first or wait for Awake().");
                return;
            }

            KillCurrentTween();

            // 先に表示状態にする
            speechBubble.SetActive(true);

            // 画面外右の位置を計算
            Vector2 offScreenRight = CalculateOffScreenRightPosition();

            // 開始位置を画面外右に設定（speechBubbleを動かす）
            _speechBubbleRect.anchoredPosition = offScreenRight;

            Debug.Log($"[TextDisplayView] EaseIn: {offScreenRight} -> {_initialPosition}");

            // 初期位置へイージング（speechBubbleを動かす）
            _currentTween = _speechBubbleRect.DOAnchorPos(_initialPosition, animationDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                _speechBubbleRect.anchoredPosition = _initialPosition;
            }
        }

        /// <summary>
        /// 現在位置から画面外右へイージングアウト（その後非表示）
        /// </summary>
        public async UniTask EaseOutToOffScreen(CancellationToken cancellationToken = default)
        {
            if (!_initialized)
            {
                Debug.LogError("[TextDisplayView] Not initialized!");
                return;
            }

            KillCurrentTween();

            // 画面外右の位置を計算
            Vector2 offScreenRight = CalculateOffScreenRightPosition();

            Debug.Log($"[TextDisplayView] EaseOut: {_speechBubbleRect.anchoredPosition} -> {offScreenRight}");

            // 画面外右へイージング（speechBubbleを動かす）
            _currentTween = _speechBubbleRect.DOAnchorPos(offScreenRight, animationDuration)
                .SetEase(Ease.InCubic)
                .SetUpdate(true);

            try
            {
                await _currentTween.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                KillCurrentTween();
                _speechBubbleRect.anchoredPosition = offScreenRight;
            }
            finally
            {
                // イージング完了後に非表示
                speechBubble.SetActive(false);
            }
        }

        /// <summary>
        /// 即座に非表示（画面外右へ移動 + 非表示）
        /// </summary>
        public void SetOffScreenImmediately()
        {
            if (!_initialized)
            {
                Debug.LogError("[TextDisplayView] Not initialized!");
                return;
            }

            KillCurrentTween();
            _speechBubbleRect.anchoredPosition = CalculateOffScreenRightPosition();
            speechBubble.SetActive(false);
        }

        /// <summary>
        /// 画面外右の座標を計算
        /// </summary>
        private Vector2 CalculateOffScreenRightPosition()
        {
            // CanvasのRectTransform
            RectTransform canvasRect = _rootCanvas.transform as RectTransform;

            // Canvas幅の取得
            float canvasWidth = canvasRect.rect.width;

            // speechBubbleの幅
            float bubbleWidth = _speechBubbleRect.rect.width;

            // speechBubbleのAnchorを考慮
            // 一般的なケース: Anchor (0.5, 0.5) または (0.5, 0) など
            // x=0 はAnchor位置基準
            // 画面右端の外に出すには、Canvas幅の半分 + Bubble幅の半分
            float offScreenX = (canvasWidth / 2f) + (bubbleWidth / 2f);

            // Y座標は初期位置を維持（エディタで設定された位置）
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

        public void SetActive(bool active)
        {
            if (!_initialized) Initialize();

            speechBubble.SetActive(active);
            if (active)
            {
                // 表示時は初期位置に配置
                _speechBubbleRect.anchoredPosition = _initialPosition;
            }
        }

        public void SetCharacterName(string name)
        {
            nameText.text = name;
            nameText.ForceMeshUpdate();
        }

        public void SetBodyText(string text)
        {
            bodyText.text = text;
            bodyText.ForceMeshUpdate();
        }

        public void SetVisibleCharacters(int count)
        {
            bodyText.maxVisibleCharacters = count;
        }

        public void SetSkipIconActive(bool active)
        {
            skipIcon.SetActive(active);
        }

        public int GetTotalCharacterCount()
        {
            bodyText.ForceMeshUpdate();
            return bodyText.GetParsedText().Length;
        }

        /// <summary>
        /// UIの上にポインターがある場合の入力無視判定
        /// </summary>
        public bool ShouldIgnoreInput()
        {
            if (EventSystem.current == null) return false;
            if (!EventSystem.current.IsPointerOverGameObject()) return false;

            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("ExclusionUI"))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            KillCurrentTween();
        }
    }
}
