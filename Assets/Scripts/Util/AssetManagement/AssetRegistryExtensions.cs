using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace AssetManagement
{
    /// <summary>
    /// AssetLoader拡張メソッド
    /// </summary>
    public static class AssetLoaderExtensions
    {
        // ========== Enum + ファイル名 ==========

        public static async UniTask<T> LoadAsync<T>(this IAssetLoader loader, Enum category, string fileName)
            where T : UnityEngine.Object
        {
            var path = AssetPathManager.GetPath(category, fileName);
            if (string.IsNullOrEmpty(path)) return null;
            return await loader.LoadAsync<T>(path);
        }

        public static T LoadSync<T>(this IAssetLoader loader, Enum category, string fileName)
            where T : UnityEngine.Object
        {
            var path = AssetPathManager.GetPath(category, fileName);
            if (string.IsNullOrEmpty(path)) return null;
            return loader.LoadSync<T>(path);
        }

        public static void Unload(this IAssetLoader loader, Enum category, string fileName)
        {
            var path = AssetPathManager.GetPath(category, fileName);
            if (!string.IsNullOrEmpty(path))
                loader.Unload(path);
        }

        // ========== レジストリ + キー ==========

        public static async UniTask<T> LoadAsync<T>(this IAssetLoader loader, AssetRegistry registry, string key)
            where T : UnityEngine.Object
        {
            var path = registry.GetPath(key);
            if (string.IsNullOrEmpty(path)) return null;
            return await loader.LoadAsync<T>(path);
        }

        public static T LoadSync<T>(this IAssetLoader loader, AssetRegistry registry, string key)
            where T : UnityEngine.Object
        {
            var path = registry.GetPath(key);
            if (string.IsNullOrEmpty(path)) return null;
            return loader.LoadSync<T>(path);
        }

        public static void Unload(this IAssetLoader loader, AssetRegistry registry, string key)
        {
            var path = registry.GetPath(key);
            if (!string.IsNullOrEmpty(path))
                loader.Unload(path);
        }
    }
}
