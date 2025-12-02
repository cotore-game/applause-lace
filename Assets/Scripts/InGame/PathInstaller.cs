using AssetManagement;

public class PathInstaller : IAssetPathInstaller
{
    public void Install(IPathRegister pathRegister)
    {
        pathRegister.Register(InGameAssetPath.TestA, "TestA");
        pathRegister.Register(InGameAssetPath.TestB, "TestA/TestB");
    }
}

public enum InGameAssetPath
{
    TestA,
    TestB,
}
