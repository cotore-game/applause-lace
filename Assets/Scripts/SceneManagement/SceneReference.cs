using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SceneManagement
{
    /// <summary>
    /// EditorではSceneAssetを参照し、実行時にはBuild Settingsで利用できるシーンパスを提供します。
    /// </summary>
    [Serializable]
    public sealed class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
#endif
        [SerializeField, HideInInspector] private string scenePath;

        /// <summary>UnityのシーンロードAPIへ渡すプロジェクト相対パスです。</summary>
        public string Path => scenePath;

        /// <summary>拡張子を除いたシーン名です。</summary>
        public string Name => string.IsNullOrWhiteSpace(scenePath)
            ? string.Empty
            : System.IO.Path.GetFileNameWithoutExtension(scenePath);
        /// <summary>ロードに使用できるパスを保持しているかを示します。</summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(scenePath);

        /// <inheritdoc />
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            scenePath = sceneAsset == null
                ? string.Empty
                : AssetDatabase.GetAssetPath(sceneAsset);
#endif
        }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
        }
    }
}
