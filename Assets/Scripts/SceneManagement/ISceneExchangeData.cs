namespace SceneManagement
{
    /// <summary>
    /// シーン間でやり取りするデータクラスが実装するマーカーインターフェース。
    /// </summary>
    /// <remarks>
    /// 使用例:
    /// <code>
    /// public class GamePlayData : ISceneExchangeData
    /// {
    ///     public string PlayerName { get; set; }
    ///     public int Level { get; set; }
    /// }
    /// </code>
    /// </remarks>
    public interface ISceneExchangeData
    {
    }
}
