using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;
using System;

public class ClapStageView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clapCountText;
    [SerializeField] private TextMeshProUGUI resultText;
    [FormerlySerializedAs("stopButton")]
    [SerializeField] private Button clapButton;

    public event Action OnClapButtonClicked;

    private void Awake()
    {
        clapButton.onClick.AddListener(HandleClapButtonClicked);
        resultText.text = "";
    }

    private void OnDestroy()
    {
        clapButton.onClick.RemoveListener(HandleClapButtonClicked);
    }

    private void HandleClapButtonClicked()
    {
        OnClapButtonClicked?.Invoke();
    }

    public void UpdateClapCount(int count)
    {
        clapCountText.text = $"Clap: {count}";
    }

    public void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
        clapButton.interactable = false;
    }

    public void ResetView()
    {
        clapCountText.text = "Clap: 0";
        resultText.text = "";
        clapButton.interactable = true;
    }
}
