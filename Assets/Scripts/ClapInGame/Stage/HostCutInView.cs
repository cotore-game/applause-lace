using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class HostCutInView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string showTrigger = "Show";
    [SerializeField, Min(0f)] private float displaySeconds = 0.5f;
    [SerializeField] private bool hideAfterDisplay = true;

    public void Prepare()
    {
        gameObject.SetActive(false);
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken)
    {
        gameObject.SetActive(true);

        if (animator != null && !string.IsNullOrWhiteSpace(showTrigger))
        {
            animator.ResetTrigger(showTrigger);
            animator.SetTrigger(showTrigger);
        }

        try
        {
            if (displaySeconds > 0f)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(displaySeconds),
                    cancellationToken: cancellationToken);
            }
        }
        finally
        {
            if (hideAfterDisplay)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
