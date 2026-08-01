using System.Threading;
using Cysharp.Threading.Tasks;

namespace SceneManagement
{
    /// <summary>
    /// Additiveロードされた子シーンが、GameCoreのGameFlowと同期するための通知口です。
    /// </summary>
    public interface ISceneFlowPort
    {
        /// <summary>指定シーンが現在GameFlowから起動されているかを返します。</summary>
        bool IsManaged(SceneId sceneId);

        /// <summary>シーン内の初期化が完了し、開始可能になったことを通知します。</summary>
        void NotifyReady(SceneId sceneId);

        /// <summary>幕などの遷移演出が完了し、コンテンツを開始できるまで待機します。</summary>
        UniTask WaitForEnterAsync(SceneId sceneId, CancellationToken cancellationToken);

        /// <summary>シーン固有の進行が完了したことを通知します。</summary>
        void NotifyComplete(SceneId sceneId);
    }
}
