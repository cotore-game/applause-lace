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
        SetImageVisible(countdownImage, false);
        SetImageVisible(celebrateImage, false);

        gameObject.SetActive(false);
    }

    public void ShowCount(int count)
    {
        gameObject.SetActive(true);

        SetImageVisible(celebrateImage, false);

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
        SetImageVisible(countdownImage, countdownImage.sprite != null);
    }

    public void ShowCelebrate()
    {
        gameObject.SetActive(true);

        SetImageVisible(countdownImage, false);
        SetImageVisible(celebrateImage, true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private static void SetImageVisible(Image image, bool visible)
    {
        if (image == null)
        {
            return;
        }

        image.gameObject.SetActive(visible);
        image.enabled = visible;
    }
}
