using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using AssetManagement;

namespace ADV.Presentation
{
    /// <summary>
    /// 単体キャラクター立ち絵を管理するプレゼンター
    /// ResourcesAssetLoaderを使用してスプライトをロード
    /// </summary>
    public class CharacterPresenter : IDisposable
    {
        private readonly CharacterView _characterView;
        private readonly ResourcesAssetLoader _assetLoader;

        private CancellationTokenSource _cts;
        private const float DEFAULT_EASE_DURATION = 0.5f;

        // 現在表示中のキャラクター情報
        private string _currentCharacterName;
        private string _currentExpression;

        public CharacterPresenter(CharacterView characterView)
        {
            _characterView = characterView ?? throw new ArgumentNullException(nameof(characterView));
            _assetLoader = ResourcesAssetLoader.Instance;
            _cts = new CancellationTokenSource();

            // 初期状態は非表示
            _characterView.HideImmediate();
        }

        /// <summary>
        /// キャラクターを表示（イージングイン）
        /// </summary>
        /// <param name="characterName">Graduate, Host, Pianist, Scientist のいずれか</param>
        /// <param name="expression">表情名（ファイル名）</param>
        /// <param name="useEasing">イージング使用するか（falseの場合は即座に表示）</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ShowCharacter(
            string characterName,
            string expression,
            bool useEasing = true,
            CancellationToken cancellationToken = default)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
            var token = linkedCts.Token;

            // アセットパスのEnum取得
            if (!TryParseCharacterName(characterName, out var assetPath))
            {
                Debug.LogError($"[CharacterPresenter] Invalid character name: {characterName}");
                return;
            }

            // スプライトをロード
            var sprite = await _assetLoader.LoadAsync<Sprite>(assetPath, expression);

            if (sprite == null)
            {
                Debug.LogError($"[CharacterPresenter] Failed to load sprite: {characterName}/{expression}");
                return;
            }

            // スプライトを設定
            _characterView.SetSprite(sprite);

            // 現在の情報を更新
            _currentCharacterName = characterName;
            _currentExpression = expression;

            // イージングイン
            if (useEasing)
            {
                await _characterView.EaseIn(DEFAULT_EASE_DURATION, token);
            }
            else
            {
                _characterView.ShowImmediate();
            }
        }

        /// <summary>
        /// 表情を切り替え（キャラクターは同じ）
        /// </summary>
        /// <param name="expression">新しい表情名</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask ChangeExpression(
            string expression,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_currentCharacterName))
            {
                Debug.LogWarning("[CharacterPresenter] No character is currently displayed");
                return;
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
            var token = linkedCts.Token;

            // アセットパスのEnum取得
            if (!TryParseCharacterName(_currentCharacterName, out var assetPath))
            {
                Debug.LogError($"[CharacterPresenter] Invalid character name: {_currentCharacterName}");
                return;
            }

            // スプライトをロード
            var sprite = await _assetLoader.LoadAsync<Sprite>(assetPath, expression);

            if (sprite == null)
            {
                Debug.LogError($"[CharacterPresenter] Failed to load sprite: {_currentCharacterName}/{expression}");
                return;
            }

            // スプライトを切り替え（即座に）
            _characterView.SetSprite(sprite);
            _currentExpression = expression;
        }

        /// <summary>
        /// キャラクターを非表示（イージングアウト）
        /// </summary>
        /// <param name="useEasing">イージング使用するか（falseの場合は即座に非表示）</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask HideCharacter(
            bool useEasing = true,
            CancellationToken cancellationToken = default)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
            var token = linkedCts.Token;

            if (useEasing)
            {
                await _characterView.EaseOut(DEFAULT_EASE_DURATION, token);
            }
            else
            {
                _characterView.HideImmediate();
            }

            // 現在の情報をクリア
            _currentCharacterName = null;
            _currentExpression = null;
        }

        /// <summary>
        /// 文字列からInGameAssetPathへ変換
        /// </summary>
        private bool TryParseCharacterName(string characterName, out InGameAssetPath assetPath)
        {
            return Enum.TryParse(characterName, true, out assetPath);
        }

        /// <summary>
        /// キャッシュクリア（AssetLoaderのキャッシュをクリア）
        /// </summary>
        public void ClearCache()
        {
            _assetLoader.ClearCache();
            Debug.Log("[CharacterPresenter] Asset cache cleared");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
