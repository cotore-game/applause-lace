using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class DialogueView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject presentationRoot;
    [SerializeField] private ADVTextDisplayView textDisplayView;
    [SerializeField] private CharacterView characterView;
    [SerializeField] private Button advanceButton;
    [SerializeField] private Animator animator;
    [SerializeField] private string showLineTrigger = "ShowLine";

    private bool _advanceRequested;

    private void Awake()
    {
        Hide();
    }

    public async UniTask ShowLineAsync(
        string speaker,
        string text,
        Sprite portrait,
        CancellationToken cancellationToken)
    {
        SetVisible(true);
        _advanceRequested = false;

        textDisplayView?.SetText(speaker, text);
        characterView?.SetPortrait(portrait);

        if (animator != null && !string.IsNullOrWhiteSpace(showLineTrigger))
        {
            animator.ResetTrigger(showLineTrigger);
            animator.SetTrigger(showLineTrigger);
        }

        advanceButton?.onClick.AddListener(RequestAdvance);

        try
        {
            await UniTask.WaitUntil(
                () => _advanceRequested,
                cancellationToken: cancellationToken);
        }
        finally
        {
            advanceButton?.onClick.RemoveListener(RequestAdvance);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RequestAdvance();
        }
    }

    public void Hide()
    {
        textDisplayView?.Clear();
        characterView?.ClearPortrait();
        SetVisible(false);
    }

    private void RequestAdvance()
    {
        _advanceRequested = true;
    }

    private void SetVisible(bool visible)
    {
        GameObject target = presentationRoot != null ? presentationRoot : gameObject;
        target.SetActive(visible);
    }
}
