using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// CSVファイルなどをシーンに渡すためのデータ
    /// </summary>
    [CreateAssetMenu(fileName = "NewCsvData", menuName = "SceneFlow/Data/CSV Data")]
    public class CsvSceneDataSO : ScriptableSceneData
    {
        [Header("Payload")]
        public TextAsset CsvFile;
        public string Description;
        // その他、このシーンに必要なパラメータを自由に定義
    }
}
