using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace SceneManagement
{
    /// <summary>
    /// シーン遷移を実行するシングルトンクラス
    /// Single / Additive ロード両対応
    /// 非同期ロードの CancellationToken 対応
    /// 二重遷移防止（Single モード）
    /// Additive 並列ロードの競合対策
    /// </summary>
    public class SceneTransitioner : SingletonMonoBehaviour<SceneTransitioner>
    {
        /// <summary>
        /// シーン遷移開始時に発火するイベント。
        /// パラメータ: (SceneId, シーン名, データ型名, LoadSceneMode)
        /// </summary>
        public event Action<SceneId, string, string, LoadSceneMode> OnTransitionStarted;

        /// <summary>
        /// シーン遷移完了時に発火するイベント。
        /// パラメータ: (SceneId, シーン名, LoadSceneMode)
        /// </summary>
        public event Action<SceneId, string, LoadSceneMode> OnTransitionCompleted;

        // 遷移中のシーン名セット（Additive並列ロード対応）
        private readonly HashSet<string> _pendingSceneNames = new HashSet<string>();

        // アクティブな遷移数（Additive では複数が同時進行する）
        private int _activeTransitionCount = 0;

        /// <summary>
        /// 現在遷移中かどうか。
        /// Single モードでは遷移前に必ず確認してください。
        /// </summary>
        public bool IsTransitioning => _activeTransitionCount > 0;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (!SceneRegistry.ValidateRegistry())
            {
                Debug.LogError("[SceneTransitioner] SceneRegistry validation failed!");
            }

            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        /// <summary>
        /// データなしで同期シーン遷移を実行します。
        /// </summary>
        public void TransitionTo(SceneId sceneId, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!TryGetSceneName(sceneId, out string sceneName)) return;
            if (!GuardSingleTransition(mode)) return;

            Debug.Log($"[SceneTransitioner] TransitionTo '{sceneId}' ({sceneName}) [{mode}]");

            BeginTransition(sceneName, sceneId, "None", mode);

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] LoadScene failed: {ex.Message}");
                AbortTransition(sceneName);
            }
        }

        /// <summary>
        /// データありで同期シーン遷移を実行します。
        /// </summary>
        public void TransitionTo(SceneId sceneId, ISceneExchangeData data, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!TryGetSceneName(sceneId, out string sceneName)) return;
            if (!GuardSingleTransition(mode)) return;

            SceneExchangeManager.Instance.StoreData(data);
            string dataTypeName = data.GetType().Name;

            Debug.Log($"[SceneTransitioner] TransitionTo '{sceneId}' ({sceneName}) with '{dataTypeName}' [{mode}]");

            BeginTransition(sceneName, sceneId, dataTypeName, mode);

            try
            {
                SceneManager.LoadScene(sceneName, mode);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] LoadScene failed: {ex.Message}");
                SceneExchangeManager.Instance.ClearData(data);
                AbortTransition(sceneName);
            }
        }

        /// <summary>
        /// データなしで非同期シーン遷移を実行します。
        /// </summary>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="mode">読み込みモード（Single / Additive）</param>
        /// <param name="progress">進捗コールバック (0.0 〜 1.0)</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask TransitionToAsync(
            SceneId sceneId,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryGetSceneName(sceneId, out string sceneName)) return;
            if (!GuardSingleTransition(mode)) return;

            Debug.Log($"[SceneTransitioner] TransitionToAsync '{sceneId}' ({sceneName}) [{mode}]");

            BeginTransition(sceneName, sceneId, "None", mode);

            try
            {
                await SceneManager.LoadSceneAsync(sceneName, mode)
                    .ToUniTask(progress, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[SceneTransitioner] Transition to '{sceneId}' was cancelled.");
                AbortTransition(sceneName);
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] LoadSceneAsync failed: {ex.Message}");
                AbortTransition(sceneName);
            }
        }

        /// <summary>
        /// データありで非同期シーン遷移を実行します。
        /// </summary>
        /// <typeparam name="TData">遷移時に渡すデータの型</typeparam>
        /// <param name="sceneId">遷移先のシーンID</param>
        /// <param name="data">遷移先に渡すデータ</param>
        /// <param name="mode">読み込みモード（Single / Additive）</param>
        /// <param name="progress">進捗コールバック (0.0 〜 1.0)</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask TransitionToAsync<TData>(
            SceneId sceneId,
            TData data,
            LoadSceneMode mode = LoadSceneMode.Single,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
            where TData : ISceneExchangeData
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (!TryGetSceneName(sceneId, out string sceneName)) return;
            if (!GuardSingleTransition(mode)) return;

            SceneExchangeManager.Instance.StoreData(data);
            string dataTypeName = typeof(TData).Name;

            Debug.Log($"[SceneTransitioner] TransitionToAsync '{sceneId}' ({sceneName}) with '{dataTypeName}' [{mode}]");

            BeginTransition(sceneName, sceneId, dataTypeName, mode);

            try
            {
                await SceneManager.LoadSceneAsync(sceneName, mode)
                    .ToUniTask(progress, cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[SceneTransitioner] Transition to '{sceneId}' was cancelled.");
                SceneExchangeManager.Instance.ClearData(data);
                AbortTransition(sceneName);
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] LoadSceneAsync failed: {ex.Message}");
                SceneExchangeManager.Instance.ClearData(data);
                AbortTransition(sceneName);
            }
        }

        /// <summary>
        /// 指定したシーンを非同期でアンロードします。
        /// </summary>
        /// <param name="sceneId">アンロードするシーンID</param>
        /// <param name="progress">進捗コールバック</param>
        /// <param name="cancellationToken">キャンセルトークン</param>
        public async UniTask UnloadSceneAsync(
            SceneId sceneId,
            IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (!TryGetSceneName(sceneId, out string sceneName)) return;

            Debug.Log($"[SceneTransitioner] UnloadSceneAsync '{sceneId}' ({sceneName})");

            try
            {
                await SceneManager.UnloadSceneAsync(sceneName)
                    .ToUniTask(progress, cancellationToken: cancellationToken);

                Debug.Log($"[SceneTransitioner] Unloaded: '{sceneId}' ({sceneName})");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[SceneTransitioner] Unload of '{sceneId}' was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneTransitioner] UnloadSceneAsync failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 指定したシーンが現在ロードされているかを確認します。
        /// </summary>
        public bool IsSceneLoaded(SceneId sceneId)
        {
            if (!TryGetSceneName(sceneId, out string sceneName)) return false;
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }

        #region Private Methods

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_pendingSceneNames.Remove(scene.name))
            {
                _activeTransitionCount = Math.Max(0, _activeTransitionCount - 1);

                SceneId? sceneId = SceneRegistry.GetSceneId(scene.name);
                if (sceneId.HasValue)
                {
                    OnTransitionCompleted?.Invoke(sceneId.Value, scene.name, mode);
                }

                Debug.Log($"[SceneTransitioner] Scene loaded: {scene.name} [{mode}] (remaining: {_activeTransitionCount})");
            }
        }

        /// <summary>
        /// Single モード時の二重遷移ガード。
        /// Additive は並列遷移を許容するためガードしない。
        /// </summary>
        private bool GuardSingleTransition(LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single && IsTransitioning)
            {
                Debug.LogWarning("[SceneTransitioner] Already transitioning in Single mode. Ignored.");
                return false;
            }
            return true;
        }

        private void BeginTransition(string sceneName, SceneId sceneId, string dataTypeName, LoadSceneMode mode)
        {
            _pendingSceneNames.Add(sceneName);
            _activeTransitionCount++;

            SceneId? id = SceneRegistry.GetSceneId(sceneName);
            OnTransitionStarted?.Invoke(sceneId, sceneName, dataTypeName, mode);
        }

        private void AbortTransition(string sceneName)
        {
            if (_pendingSceneNames.Remove(sceneName))
            {
                _activeTransitionCount = Math.Max(0, _activeTransitionCount - 1);
            }
        }

        private bool TryGetSceneName(SceneId sceneId, out string sceneName)
        {
            sceneName = SceneRegistry.GetSceneName(sceneId);
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"[SceneTransitioner] No scene name found for SceneId: {sceneId}");
                return false;
            }
            return true;
        }

        #endregion
    }
}
