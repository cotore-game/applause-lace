using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// シーン間のデータ受け渡しを管理するシングルトンクラス。
    /// SceneTransitioner と同一 Assembly に置くことを前提とし、StoreData は internal です。
    /// </summary>
    public class SceneExchangeManager : SingletonMonoBehaviour<SceneExchangeManager>
    {
        private readonly Dictionary<Type, ISceneExchangeData> _dataStorage = new Dictionary<Type, ISceneExchangeData>();

        /// <summary>
        /// シーン遷移用のデータを格納します。
        /// SceneTransitioner から内部的に呼ばれます。直接呼ばないでください。
        /// </summary>
        internal void StoreData(ISceneExchangeData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Cannot store null data.");

            Type dataType = data.GetType();
            _dataStorage[dataType] = data;
            Debug.Log($"[SceneExchangeManager] Stored: {dataType.Name}");
        }

        /// <summary>
        /// 格納されたデータを取得し、ストレージから削除します。
        /// データが存在しない場合は null を返します。
        /// </summary>
        public TData GetData<TData>() where TData : class, ISceneExchangeData
        {
            Type dataType = typeof(TData);

            if (_dataStorage.TryGetValue(dataType, out ISceneExchangeData data))
            {
                _dataStorage.Remove(dataType);
                Debug.Log($"[SceneExchangeManager] Retrieved and cleared: {dataType.Name}");
                return (TData)data;
            }

            Debug.LogWarning($"[SceneExchangeManager] No data found for type: {dataType.Name}. Returning null.");
            return default;
        }

        /// <summary>
        /// 格納されたデータを取得しますが、ストレージからは削除しません。
        /// </summary>
        public TData PeekData<TData>() where TData : class, ISceneExchangeData
        {
            Type dataType = typeof(TData);
            return _dataStorage.TryGetValue(dataType, out ISceneExchangeData data) ? (TData)data : default;
        }

        /// <summary>
        /// 指定した型のデータがストレージに存在するかを確認します。
        /// </summary>
        public bool HasData<TData>() where TData : class, ISceneExchangeData
            => _dataStorage.ContainsKey(typeof(TData));

        /// <summary>
        /// 指定した型のデータをストレージから削除します。（インスタンスベース・内部用）
        /// 同期 TransitionTo のロールバック用。
        /// </summary>
        internal bool ClearData(ISceneExchangeData data)
        {
            if (data == null) return false;
            Type dataType = data.GetType();
            bool removed = _dataStorage.Remove(dataType);
            if (removed) Debug.Log($"[SceneExchangeManager] Cleared (rollback): {dataType.Name}");
            return removed;
        }

        /// <summary>
        /// 指定した型のデータをストレージから削除します。
        /// </summary>
        public bool ClearData<TData>() where TData : class, ISceneExchangeData
        {
            Type dataType = typeof(TData);
            bool removed = _dataStorage.Remove(dataType);
            if (removed) Debug.Log($"[SceneExchangeManager] Cleared: {dataType.Name}");
            return removed;
        }

        /// <summary>
        /// ストレージ内のすべてのデータを削除します。
        /// </summary>
        public void ClearAllData()
        {
            int count = _dataStorage.Count;
            _dataStorage.Clear();
            Debug.Log($"[SceneExchangeManager] All data cleared. {count} entries removed.");
        }

        /// <summary>
        /// 現在ストレージに格納されているデータの数。
        /// </summary>
        public int StoredDataCount => _dataStorage.Count;
    }
}
