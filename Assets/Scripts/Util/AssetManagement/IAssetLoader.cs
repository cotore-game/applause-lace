using Cysharp.Threading.Tasks;

namespace AssetManagement
{
    /// <summary>
    /// アセットロード用のインターフェース
    /// </summary>
    public interface IAssetLoader
    {
        UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
        T LoadSync<T>(string path) where T : UnityEngine.Object;
        void Unload(string path);
        void UnloadAll();
        void ClearCache();
    }
}
