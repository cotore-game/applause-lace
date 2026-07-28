using UnityEngine;
using UnityEngine.UI;

public sealed class CountdownView : MonoBehaviour
{
    [Header("表示先")]
    [SerializeField] private Image countdownImage;
    [SerializeField] private Image celebrateImage;

    [Header("カウントダウン画像")]
    [SerializeField] private Sprite count3Sprite;
    [SerializeField] private Sprite count2Sprite;
    [SerializeField] private Sprite count1Sprite;

    [Header("表示時間")]
    [SerializeField, Min(0f)] private float countIntervalSeconds = 1f;
    [SerializeField, Min(0f)] private float celebrateSeconds = 0.5f;

    public float CountIntervalSeconds => countIntervalSeconds;
    public float CelebrateSeconds => celebrateSeconds;

    public void Prepare()
    {
        if (countdownImage != null)
        {
            countdownImage.enabled = false;
        }

        if (celebrateImage != null)
        {
            celebrateImage.enabled = false;
        }

        gameObject.SetActive(false);
    }

    public void ShowCount(int count)
    {
        gameObject.SetActive(true);

        if (celebrateImage != null)
        {
            celebrateImage.enabled = false;
        }

        if (countdownImage == null)
        {
            return;
        }

        countdownImage.sprite = count switch
        {
            3 => count3Sprite,
            2 => count2Sprite,
            1 => count1Sprite,
            _ => null
        };
        countdownImage.enabled = countdownImage.sprite != null;
    }

    public void ShowCelebrate()
    {
        gameObject.SetActive(true);

        if (countdownImage != null)
        {
            countdownImage.enabled = false;
        }

        if (celebrateImage != null)
        {
            celebrateImage.enabled = true;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
