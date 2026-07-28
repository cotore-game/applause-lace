using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class StageDialogueEventView : MonoBehaviour, IDialogueEventDispatcher
{
    [SerializeField] private ClapHandButtonView clapButton;
    [SerializeField] private GameObject clapButtonSpotlight;

    private void Awake()
    {
        clapButton?.SetVisible(false);
        SetActive(clapButtonSpotlight, false);
    }

    public UniTask DispatchAsync(
        DialogueEventType eventType,
        string eventArgument,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (eventType)
        {
            case DialogueEventType.None:
                break;

            case DialogueEventType.ShowClapButton:
                clapButton?.SetVisible(true);
                break;

            case DialogueEventType.SpotlightClapButton:
                clapButton?.SetVisible(true);
                SetActive(clapButtonSpotlight, true);
                break;

            case DialogueEventType.HideClapButton:
                SetActive(clapButtonSpotlight, false);
                clapButton?.SetVisible(false);
                break;

            case DialogueEventType.PlayConfetti:
                Debug.LogWarning($"{name}: Confettiイベントは現在未実装です。");
                break;

            case DialogueEventType.Custom:
                Debug.LogWarning(
                    $"{name}: Custom ADVイベントの処理が未実装です。argument='{eventArgument}'");
                break;
        }

        return UniTask.CompletedTask;
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}
