using System;

namespace AssetManagement
{
    /// <summary>
    /// パス登録用インターフェース
    /// </summary>
    public interface IPathRegister
    {
        void Register(Enum category, string path);
    }
}
