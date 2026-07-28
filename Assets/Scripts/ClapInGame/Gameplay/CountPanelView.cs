using TMPro;
using UnityEngine;

public sealed class CountPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;

    public void SetCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
