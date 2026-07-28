using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ClapHandButtonView :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image handImage;
    [SerializeField] private Sprite openHandSprite;
    [SerializeField] private Sprite closedHandSprite;

    private bool _isPressed;

    public event Action OnClapCompleted;

    private void Awake()
    {
        ShowOpenHand();
    }

    private void OnDisable()
    {
        _isPressed = false;
        ShowOpenHand();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanReceiveInput(eventData))
        {
            return;
        }

        _isPressed = true;
        SetHandSprite(closedHandSprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool shouldClap = _isPressed && CanReceiveInput(eventData);
        _isPressed = false;
        ShowOpenHand();

        if (shouldClap)
        {
            OnClapCompleted?.Invoke();
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        if (button != null)
        {
            button.interactable = enabled;
        }

        if (!enabled)
        {
            _isPressed = false;
            ShowOpenHand();
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    private bool CanReceiveInput(PointerEventData eventData)
    {
        return isActiveAndEnabled
               && (button == null || button.interactable)
               && eventData.button == PointerEventData.InputButton.Left;
    }

    private void ShowOpenHand()
    {
        SetHandSprite(openHandSprite);
    }

    private void SetHandSprite(Sprite sprite)
    {
        if (handImage != null && sprite != null)
        {
            handImage.sprite = sprite;
        }
    }
}
