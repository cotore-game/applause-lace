using Cysharp.Threading.Tasks;
using UnityEngine;

// 仮クラス、本来ではUIを表示したりCSVを読んだりする
// インターフェース切っておいてもいいかも知れない
public class TutorialService
{
    public async UniTask PlayDialogue(string text, bool waitForInput = false)
    {
        // 実際はUI表示
        Debug.Log($"[司会者]: {text}");

        // 文字送り待機時間のシミュレーション
        await UniTask.Delay(1000);

        if (waitForInput)
        {
            Debug.Log("（クリック待ち...）");
            // 入力待ち, InputSystemにする
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0));
        }
    }
}
