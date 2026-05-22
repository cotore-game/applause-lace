namespace AssetManagement
{
    /// <summary>
    /// ユーザーが実装するアセットパス設定用インターフェース
    /// </summary>
    public interface IAssetPathInstaller
    {
        void Install(IPathRegister register);
    }
}
