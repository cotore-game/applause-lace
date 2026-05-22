using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssetManagement
{
    /// <summary>
    /// アセットパス管理 (自動的にすべてのInstallerを検出・実行)
    /// </summary>
    public static class AssetPathManager
    {
        private static readonly Dictionary<Enum, string> _pathMap = new();
        private static bool _isInitialized = false;

        /// <summary>
        /// 初期化 (すべてのIAssetPathInstallerを自動検出して実行)
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            var register = new PathRegisterImpl();

            // すべてのアセンブリからIAssetPathInstallerを実装している型を検索
            var installerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes(); 
                    }
                    catch 
                    { 
                        return Enumerable.Empty<Type>();
                    }
                })
                .Where(type => typeof(IAssetPathInstaller).IsAssignableFrom(type)
                            && !type.IsAbstract
                            && !type.IsInterface);

            // 検出したすべてのInstallerを実行
            foreach (var installerType in installerTypes)
            {
                try
                {
                    var installer = (IAssetPathInstaller)Activator.CreateInstance(installerType);
                    installer.Install(register);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AssetPathManager] Failed to create installer: {installerType.Name}\n{e}");
                }
            }

            _isInitialized = true;
            Debug.Log($"[AssetPathManager] Initialized with {_pathMap.Count} paths");
        }

        /// <summary>
        /// Enumからパスを取得
        /// </summary>
        public static string GetPath(Enum category)
        {
            if (!_isInitialized)
                Initialize();

            if (_pathMap.TryGetValue(category, out var path))
                return path;

            Debug.LogWarning($"[AssetPathManager] Category not registered: {category}");
            return null;
        }

        /// <summary>
        /// Enum + ファイル名でフルパスを取得
        /// </summary>
        public static string GetPath(Enum category, string fileName)
        {
            var basePath = GetPath(category);
            return string.IsNullOrEmpty(basePath) ? null : $"{basePath}/{fileName}";
        }

        /// <summary>
        /// パス登録の内部実装
        /// </summary>
        private class PathRegisterImpl : IPathRegister
        {
            public void Register(Enum category, string path)
            {
                _pathMap[category] = path;
            }
        }
    }
}
