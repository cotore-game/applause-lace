using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // アニメーション用（推奨）

public class GameView : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private Text scoreText;
    [SerializeField] private ParticleSystem confettiParticle;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button clickAreaButton;

    // Presenterが購読するためのClickイベント
    public UnityEngine.Events.UnityAction OnClickAction;

    private void Start()
    {
        clickAreaButton.onClick.AddListener(() => OnClickAction?.Invoke());
    }

    // 表示更新メソッド群

    public void SetCharacter(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Claps: {score}";
        // キャラの表情変化ロジックをここに書くか、Presenterで制御するか
        UpdateCharacterExpression(score);
    }

    public void PlayConfetti()
    {
        confettiParticle.Play();
        // ここでボタンが沈むアニメーションなどを入れると手触りが良くなるかな
    }

    public void ShowGameOver()
    {
        // 演出
        gameOverPanel.SetActive(true);
    }

    public void ShowResult(int finalScore)
    {
        // リザルト表示
    }

    private void UpdateCharacterExpression(int score) { /* ... */ }
}
