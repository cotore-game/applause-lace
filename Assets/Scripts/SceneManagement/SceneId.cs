namespace SceneManagement
{
    /// <summary>ゲームフローが扱うコンテンツシーンの論理識別子です。</summary>
    public enum SceneId
    {
        Title,
        Tutorial,
        Stage1,
        Stage2,
        Stage3,
        TotalResult
    }

    /// <summary><see cref="SceneId"/>のコンテンツ種別判定を提供します。</summary>
    public static class SceneIdExtensions
    {
        /// <summary>拍手ゲームを実行するステージシーンかを返します。</summary>
        public static bool IsClapStage(this SceneId sceneId)
        {
            return sceneId is SceneId.Stage1 or SceneId.Stage2 or SceneId.Stage3;
        }
    }
}
