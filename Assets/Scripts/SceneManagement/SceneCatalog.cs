using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    /// <summary><see cref="SceneId"/>と実際のSceneAsset参照を一元管理します。</summary>
    [CreateAssetMenu(fileName = "SceneCatalog", menuName = "Game/Scene Catalog")]
    public sealed class SceneCatalog : ScriptableObject
    {
        [Serializable]
        private sealed class Entry
        {
            [SerializeField] private SceneId id;
            [SerializeField] private SceneReference scene;

            public SceneId Id => id;
            public SceneReference Scene => scene;
        }

        [SerializeField] private List<Entry> entries = new();

        /// <summary>指定したIDに対応する、有効なシーン参照を取得します。</summary>
        /// <exception cref="InvalidOperationException">
        /// IDが未登録、またはSceneAssetが未設定の場合に送出されます。
        /// </exception>
        public SceneReference Get(SceneId id)
        {
            foreach (Entry entry in entries)
            {
                if (entry.Id == id && entry.Scene != null && entry.Scene.IsValid)
                {
                    return entry.Scene;
                }
            }

            throw new InvalidOperationException(
                $"SceneCatalogに有効なSceneReferenceが登録されていません: {id}");
        }

        private void OnValidate()
        {
            HashSet<SceneId> registeredIds = new();

            foreach (Entry entry in entries)
            {
                if (!registeredIds.Add(entry.Id))
                {
                    Debug.LogWarning(
                        $"{name}: SceneId '{entry.Id}' が重複しています。",
                        this);
                }
            }
        }
    }
}
