using System.Collections.Generic;
using UnityEngine;

namespace AssetManagement
{
    /// <summary>
    /// Inspector上でアセットパスを管理するレジストリ
    /// </summary>
    [CreateAssetMenu(fileName = "AssetRegistry", menuName = "Asset Management/Asset Registry")]
    public class AssetRegistry : ScriptableObject
    {
        [SerializeField] private List<AssetEntry> entries = new();

        private Dictionary<string, string> _cache;

        private void OnEnable()
        {
            BuildCache();
        }

        private void BuildCache()
        {
            _cache = new Dictionary<string, string>();
            foreach (var entry in entries)
            {
                if (!string.IsNullOrEmpty(entry.key))
                    _cache[entry.key] = entry.path;
            }
        }

        public string GetPath(string key)
        {
            if (_cache == null)
                BuildCache();

            if (_cache.TryGetValue(key, out var path))
                return path;

            Debug.LogWarning($"[AssetRegistry] Key not found: {key}");
            return null;
        }
    }
}
