using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;

// 仮クラス、本来ではUIを表示したりCSVを読んだりする
// インターフェース切っておいてもいいかも知れない
namespace GameDialogue
{
    /// <summary>
    /// 対話システム
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        [Header("Dialogue Box Prefab")]
        [SerializeField] private GameObject dialogueBoxPrefab;

        [Header("Character Image (Optional)")]
        [SerializeField] private Image characterImage;

        [Header("Animation Settings")]
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private float balloonInDuration = 0.3f;

        private IDialogueBox _dialogueBox;
        private CanvasGroup _canvasGroup;
        private RectTransform _balloonTransform;
        private TMP_Text _speakerNameText;
        private TMP_Text _messageText;

        private bool _isSkipRequested = false;
        private bool _isInitialized = false;

        private void Awake()
        {
            Initialize();
        }

        #region test method
        public async UniTask PlayDialogue(string text, bool waitForInput = false)
        {
            // 実際はUI表示
            Debug.Log($"[司会者]: {text}");

            // 文字送り待機時間のシミュレーション
            await UniTask.Delay(1000);

            if (waitForInput)
            {
                Debug.Log("（クリック待ち...）");
                // 入力待ち, InputSystemにする
                await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
            }
        }
        #endregion

        /// <summary>
        /// 初期化 (Prefabから自動取得)
        /// </summary>
        private void Initialize()
        {
            if (_isInitialized) return;

            if (dialogueBoxPrefab == null)
            {
                Debug.LogError("[DialogueSystem] DialogueBox Prefab is not assigned!");
                return;
            }

            // PrefabからIDialogueBoxを取得
            _dialogueBox = dialogueBoxPrefab.GetComponent<IDialogueBox>();

            if (_dialogueBox == null)
            {
                Debug.LogError("[DialogueSystem] DialogueBox Prefab does not implement IDialogueBox!");
                return;
            }

            // 各種参照を自動取得
            _canvasGroup = _dialogueBox.GetCanvasGroup();
            _balloonTransform = _dialogueBox.GetBalloonTransform();
            _speakerNameText = _dialogueBox.GetSpeakerNameText();
            _messageText = _dialogueBox.GetMessageText();

            // 初期状態は非表示
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.gameObject.SetActive(false);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// DialogueBoxを手動でセット (DI用)
        /// </summary>
        public void SetDialogueBox(IDialogueBox dialogueBox)
        {
            _dialogueBox = dialogueBox;

            _canvasGroup = _dialogueBox.GetCanvasGroup();
            _balloonTransform = _dialogueBox.GetBalloonTransform();
            _speakerNameText = _dialogueBox.GetSpeakerNameText();
            _messageText = _dialogueBox.GetMessageText();

            _isInitialized = true;
        }

        /// <summary>
        /// キャラクター画像を手動でセット (DI用)
        /// </summary>
        public void SetCharacterImage(Image image)
        {
            characterImage = image;
        }

        /// <summary>
        /// 対話を表示 (タイプライター + 吹き出しアニメ)
        /// </summary>
        public async UniTask ShowDialogue(string speakerName, string message, Sprite characterSprite = null, bool waitForInput = false)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[DialogueSystem] DialogueSystem is not initialized!");
                return;
            }

            // 立ち絵変更
            if (characterSprite != null && characterImage != null)
            {
                characterImage.sprite = characterSprite;
            }

            // 話者名設定
            if (_speakerNameText != null)
            {
                _speakerNameText.text = speakerName;
            }

            // 吹き出しイン演出
            await ShowBalloon();

            // タイプライター表示
            await TypewriterEffect(message);

            // 入力待ち
            if (waitForInput)
            {
                await WaitForInput();
            }
        }

        /// <summary>
        /// 対話を隠す
        /// </summary>
        public async UniTask HideDialogue()
        {
            if (_balloonTransform != null)
            {
                await _balloonTransform.DOScale(0f, 0.2f)
                    .SetEase(Ease.InBack);
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 吹き出しイン演出
        /// </summary>
        private async UniTask ShowBalloon()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.gameObject.SetActive(true);
            }

            if (_balloonTransform != null)
            {
                _balloonTransform.localScale = Vector3.zero;
                await _balloonTransform.DOScale(1f, balloonInDuration)
                    .SetEase(Ease.OutBack);
            }
        }

        /// <summary>
        /// タイプライター効果
        /// </summary>
        private async UniTask TypewriterEffect(string fullText)
        {
            if (_messageText == null) return;

            _messageText.text = "";
            _isSkipRequested = false;

            for (int i = 0; i <= fullText.Length; i++)
            {
                // スキップされたら即座に全文表示
                if (_isSkipRequested)
                {
                    _messageText.text = fullText;
                    break;
                }

                _messageText.text = fullText.Substring(0, i);

                // タグ文字はスキップ (リッチテキスト対応)
                if (i < fullText.Length && fullText[i] == '<')
                {
                    int endIndex = fullText.IndexOf('>', i);
                    if (endIndex > 0)
                    {
                        i = endIndex;
                        continue;
                    }
                }

                await UniTask.Delay((int)(typewriterSpeed * 1000));
            }
        }

        /// <summary>
        /// 入力待ち (クリックorキー)
        /// </summary>
        private async UniTask WaitForInput()
        {
            Debug.Log("（クリック待ち...）");

            await UniTask.WaitUntil(() =>
                Input.GetMouseButtonDown(0) ||
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return)
            );
        }

        /// <summary>
        /// タイプライタースキップ
        /// </summary>
        public void SkipTypewriter()
        {
            _isSkipRequested = true;
        }
    }
}
