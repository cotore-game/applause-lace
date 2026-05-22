using AssetManagement;

public class PathInstaller : IAssetPathInstaller
{
    public void Install(IPathRegister pathRegister)
    {
        pathRegister.Register(InGameAssetPath.Graduate, "Graduate");
        pathRegister.Register(InGameAssetPath.Host, "Host");
        pathRegister.Register(InGameAssetPath.Pianist, "Pianist");
        pathRegister.Register(InGameAssetPath.Scientist, "Scientist");
    }
}

/// <summary>
/// インゲームアセットのパス定義
/// </summary>
public enum InGameAssetPath
{
    Graduate,
    Host,
    Pianist,
    Scientist
}
