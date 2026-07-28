using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class StageAnimationView : MonoBehaviour
{
    private enum CompletionMode
    {
        Duration,
        AnimationEvent
    }

    [SerializeField] private GameObject presentationRoot;
    [SerializeField] private Animator animator;
    [SerializeField] private string playTrigger = "Play";
    [SerializeField] private CompletionMode completionMode = CompletionMode.Duration;
    [SerializeField, Min(0f)] private float duration = 0.5f;
    [SerializeField] private bool hideOnComplete = true;

    private bool _animationCompleted;
    private bool _isPlaying;

    public void Prepare()
    {
        _isPlaying = false;
        _animationCompleted = false;
        SetVisible(false);
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken)
    {
        if (_isPlaying)
        {
            Debug.LogWarning($"{name}: 同じ演出が重複して再生されました。");
            return;
        }

        _isPlaying = true;
        _animationCompleted = false;
        SetVisible(true);

        try
        {
            if (animator != null && !string.IsNullOrWhiteSpace(playTrigger))
            {
                animator.ResetTrigger(playTrigger);
                animator.SetTrigger(playTrigger);
            }

            if (completionMode == CompletionMode.AnimationEvent)
            {
                await UniTask.WaitUntil(
                    () => _animationCompleted,
                    cancellationToken: cancellationToken);
            }
            else if (duration > 0f)
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(duration),
                    cancellationToken: cancellationToken);
            }
        }
        finally
        {
            _isPlaying = false;

            if (hideOnComplete)
            {
                SetVisible(false);
            }
        }
    }

    // Animation Clipの末尾から呼び出す。
    public void CompleteAnimation()
    {
        _animationCompleted = true;
    }

    public void SetVisible(bool visible)
    {
        GameObject target = presentationRoot != null ? presentationRoot : gameObject;
        target.SetActive(visible);
    }
}
