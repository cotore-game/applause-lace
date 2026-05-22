using System;

namespace AssetManagement
{
    /// <summary>
    /// アセットレジストリ用のエントリ
    /// </summary>
    [Serializable]
    public class AssetEntry
    {
        public string key;
        public string path;
    }
}
