namespace SceneManagement
{
    /// <summary>
    /// LifetimeScope内のコンテンツが、自身の論理SceneIdを共有するための値オブジェクトです。
    /// </summary>
    public sealed class SceneIdentity
    {
        /// <summary>現在のコンテンツシーンを表すIDです。</summary>
        public SceneId Id { get; }

        /// <summary>LifetimeScopeへ登録するSceneIdを指定します。</summary>
        public SceneIdentity(SceneId id)
        {
            Id = id;
        }
    }
}
