using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SceneManagement
{
    /// <summary>
    /// GameCoreを残したまま、コンテンツシーンをAdditiveで交換するためのAPIです。
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>指定したシーンが現在ロード済みかを返します。</summary>
        bool IsLoaded(SceneId sceneId);

        /// <summary>指定したコンテンツシーンをAdditiveでロードします。</summary>
        UniTask LoadAdditiveAsync(
            SceneId sceneId,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);

        /// <summary>指定したコンテンツシーンをアンロードします。</summary>
        UniTask UnloadAsync(
            SceneId sceneId,
            CancellationToken cancellationToken,
            IProgress<float> progress = null);
    }
}
