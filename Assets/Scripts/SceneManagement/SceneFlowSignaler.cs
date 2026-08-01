using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SceneManagement
{
    /// <summary>
    /// GameFlowとAdditiveロードされた子シーンのReady・Enter・Complete同期を仲介します。
    /// </summary>
    public sealed class SceneFlowSignaler : ISceneFlowPort
    {
        private sealed class Session
        {
            public readonly UniTaskCompletionSource Ready = new();
            public readonly UniTaskCompletionSource Enter = new();
            public readonly UniTaskCompletionSource Complete = new();
        }

        private readonly Dictionary<SceneId, Session> _sessions = new();

        /// <inheritdoc />
        public bool IsManaged(SceneId sceneId) => _sessions.ContainsKey(sceneId);

        /// <inheritdoc />
        public void NotifyReady(SceneId sceneId)
        {
            GetSession(sceneId).Ready.TrySetResult();
        }

        /// <inheritdoc />
        public UniTask WaitForEnterAsync(
            SceneId sceneId,
            CancellationToken cancellationToken)
        {
            return GetSession(sceneId).Enter.Task
                .AttachExternalCancellation(cancellationToken);
        }

        /// <inheritdoc />
        public void NotifyComplete(SceneId sceneId)
        {
            GetSession(sceneId).Complete.TrySetResult();
        }

        internal void Begin(SceneId sceneId)
        {
            if (!_sessions.TryAdd(sceneId, new Session()))
            {
                throw new InvalidOperationException(
                    $"シーンフローはすでに開始されています: {sceneId}");
            }
        }

        internal UniTask WaitUntilReadyAsync(
            SceneId sceneId,
            CancellationToken cancellationToken)
        {
            return GetSession(sceneId).Ready.Task
                .AttachExternalCancellation(cancellationToken);
        }

        internal void AllowEnter(SceneId sceneId)
        {
            GetSession(sceneId).Enter.TrySetResult();
        }

        internal UniTask WaitUntilCompleteAsync(
            SceneId sceneId,
            CancellationToken cancellationToken)
        {
            return GetSession(sceneId).Complete.Task
                .AttachExternalCancellation(cancellationToken);
        }

        internal void End(SceneId sceneId)
        {
            _sessions.Remove(sceneId);
        }

        private Session GetSession(SceneId sceneId)
        {
            if (_sessions.TryGetValue(sceneId, out Session session))
            {
                return session;
            }

            throw new InvalidOperationException(
                $"GameFlowControllerが管理していないシーンです: {sceneId}");
        }
    }
}
