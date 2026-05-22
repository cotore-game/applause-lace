using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace AssetManagement
{
    /// <summary>
    /// Resources用のアセットローダー
    /// </summary>
    public class ResourcesAssetLoader : IAssetLoader
    {
        private static ResourcesAssetLoader _instance;
        public static ResourcesAssetLoader Instance => _instance ??= new ResourcesAssetLoader();

        private readonly Dictionary<string, UnityEngine.Object> _cache = new();
        private readonly Dictionary<string, int> _referenceCount = new();

        public bool EnableCache { get; set; } = true;
        public bool EnableLog { get; set; } = false;

        private ResourcesAssetLoader() { }

        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                LogError($"Path is null or empty");
                return null;
            }

            if (EnableCache && _cache.TryGetValue(path, out var cached))
            {
                IncrementReference(path);
                Log($"Loaded from cache: {path}");
                return cached as T;
            }

            var request = Resources.LoadAsync<T>(path);
            await request;

            if (request.asset == null)
            {
                LogError($"Failed to load asset at path: {path}");
                return null;
            }

            var asset = request.asset as T;

            if (EnableCache)
            {
                _cache[path] = asset;
                IncrementReference(path);
            }

            Log($"Loaded: {path}");
            return asset;
        }

        public T LoadSync<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                LogError($"Path is null or empty");
                return null;
            }

            if (EnableCache && _cache.TryGetValue(path, out var cached))
            {
                IncrementReference(path);
                Log($"Loaded from cache: {path}");
                return cached as T;
            }

            var asset = Resources.Load<T>(path);

            if (asset == null)
            {
                LogError($"Failed to load asset at path: {path}");
                return null;
            }

            if (EnableCache)
            {
                _cache[path] = asset;
                IncrementReference(path);
            }

            Log($"Loaded: {path}");
            return asset;
        }

        public void Unload(string path)
        {
            if (!_cache.ContainsKey(path)) return;

            DecrementReference(path);

            if (_referenceCount.TryGetValue(path, out var count) && count <= 0)
            {
                _cache.Remove(path);
                _referenceCount.Remove(path);
                Log($"Unloaded: {path}");
            }
        }

        public void UnloadAll()
        {
            _cache.Clear();
            _referenceCount.Clear();
            Resources.UnloadUnusedAssets();
            Log("Unloaded all assets");
        }

        public void ClearCache()
        {
            _cache.Clear();
            _referenceCount.Clear();
            Log("Cache cleared");
        }

        private void IncrementReference(string path)
        {
            if (!_referenceCount.ContainsKey(path))
                _referenceCount[path] = 0;
            _referenceCount[path]++;
        }

        private void DecrementReference(string path)
        {
            if (_referenceCount.ContainsKey(path))
            {
                _referenceCount[path]--;
                if (_referenceCount[path] < 0)
                    _referenceCount[path] = 0;
            }
        }

        private void Log(string message)
        {
            if (EnableLog)
                Debug.Log($"[ResourcesAssetLoader] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[ResourcesAssetLoader] {message}");
        }
    }
}
