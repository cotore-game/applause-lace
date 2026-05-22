#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EditorSceneBootstrap
{
    private const string CoreSceneName = "GameCore";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // すでにGameCoreが読み込まれているかチェック
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == CoreSceneName) return;
        }

        // 存在しなければAdditiveでロード
        SceneManager.LoadScene(CoreSceneName, LoadSceneMode.Additive);
    }
}
#endif
