using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    /// <inheritdoc />
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly SceneCatalog _catalog;

        /// <summary>SceneIdの解決に使用するカタログを受け取ります。</summary>
        public SceneLoader(SceneCatalog catalog)
        {
            _catalog = catalog;
        }

        /// <inheritdoc />
        public bool IsLoaded(SceneId sceneId)
        {
            SceneReference sceneReference = _catalog.Get(sceneId);
            return SceneManager.GetSceneByPath(sceneReference.Path).isLoaded;
        }

        /// <inheritdoc />
        public async UniTask LoadAdditiveAsync(
            SceneId sceneId,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SceneReference sceneReference = _catalog.Get(sceneId);

            if (SceneManager.GetSceneByPath(sceneReference.Path).isLoaded)
            {
                progress?.Report(1f);
                return;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(
                sceneReference.Path,
                LoadSceneMode.Additive);

            if (operation == null)
            {
                throw new InvalidOperationException(
                    $"シーンのロードを開始できませんでした: {sceneId} ({sceneReference.Path})");
            }

            await operation.ToUniTask(
                progress: progress);
            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <inheritdoc />
        public async UniTask UnloadAsync(
            SceneId sceneId,
            CancellationToken cancellationToken,
            IProgress<float> progress = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SceneReference sceneReference = _catalog.Get(sceneId);
            Scene scene = SceneManager.GetSceneByPath(sceneReference.Path);

            if (!scene.isLoaded)
            {
                progress?.Report(1f);
                return;
            }

            AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);

            if (operation == null)
            {
                throw new InvalidOperationException(
                    $"シーンのアンロードを開始できませんでした: {sceneId} ({sceneReference.Path})");
            }

            await operation.ToUniTask(
                progress: progress);
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
