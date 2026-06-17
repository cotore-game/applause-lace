#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using VContainer;

/// <summary>
/// テストシーンのGameObjectにアタッチして使用するテスト用コンポーネント
/// </summary>
public class CurtainControllerTest : MonoBehaviour
{
    // インスペクターから直接設定するか、VContainerから注入します
    [SerializeField] private CurtainController curtainController;

    private bool _isAnimating;

    [Inject]
    public void Construct(CurtainController controller)
    {
        // VContainerから注入された場合はこちらを優先
        if (curtainController == null)
        {
            curtainController = controller;
        }
    }

    private void Update()
    {
        if (_isAnimating) return;
        if (curtainController == null) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // Oキーで幕を開く
        if (keyboard.oKey.wasPressedThisFrame)
        {
            TriggerOpenAsync().Forget();
        }
        // Cキーで幕を閉じる
        else if (keyboard.cKey.wasPressedThisFrame)
        {
            TriggerCloseAsync().Forget();
        }
        // Rキーで即座にリセット
        else if (keyboard.rKey.wasPressedThisFrame)
        {
            curtainController.ResetCurtainPosition();
            Debug.Log("[Test] Curtain Position Reset.");
        }
    }

    private async UniTaskVoid TriggerOpenAsync()
    {
        _isAnimating = true;
        Debug.Log("[Test] OpenCurtainAsync Start.");
        await curtainController.OpenCurtainAsync();
        Debug.Log("[Test] OpenCurtainAsync Complete.");
        _isAnimating = false;
    }

    private async UniTaskVoid TriggerCloseAsync()
    {
        _isAnimating = true;
        Debug.Log("[Test] CloseCurtainAsync Start.");
        await curtainController.CloseCurtainAsync();
        Debug.Log("[Test] CloseCurtainAsync Complete.");
        _isAnimating = false;
    }
}
#endif
