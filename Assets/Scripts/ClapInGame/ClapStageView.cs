using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ClapStageView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clapCountText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button stopButton;

    // ボタンが押されたことをPresenterに通知するためのイベント
    public event Action OnStopButtonClicked;

    private void Awake()
    {
        stopButton.onClick.AddListener(() => OnStopButtonClicked?.Invoke());
        resultText.text = ""; // 初期化
    }

    public void UpdateClapCount(int count)
    {
        clapCountText.text = $"Clap: {count}";
    }

    public void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
        stopButton.interactable = false; // 終了したらボタンを押せなくする
    }

    public void ResetView()
    {
        clapCountText.text = "Clap: 0";
        resultText.text = "";
        stopButton.interactable = true;
    }
}
