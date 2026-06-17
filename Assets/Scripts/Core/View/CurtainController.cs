using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 舞台幕の開閉と背景切り替えを制御するコントローラー
/// </summary>
public class CurtainController : MonoBehaviour
{
    [Header("幕の参照")]
    [SerializeField] private RectTransform mainCurtain; // 大幕
    [SerializeField] private RectTransform topDecoration; // 上部装飾カーテン

    [Header("ポジション設定")]
    [SerializeField] private RectTransform lowerClosedTarget; // 大幕・閉位置
    [SerializeField] private RectTransform lowerOpenTarget; // 大幕・開位置
    [SerializeField] private RectTransform upperClosedTarget; // 装飾・閉位置
    [SerializeField] private RectTransform upperOpenTarget; // 装飾・開位置

    [Header("アニメーション設定")]
    [SerializeField] private float anticipationDuration = 0.3f; // 予備動作の時間
    [SerializeField] private float anticipationDistance = 50f; // 予備動作の距離（下方向）
    [SerializeField] private float mainCurtainUpDuration = 1.0f; // 大幕が上がる時間
    [SerializeField] private float topDecorationUpDuration = 0.8f; // 装飾が上がる時間
    [SerializeField] private float topDecorationDelay = 0.5f; // 装飾が動き始めるまでの遅延
    [SerializeField] private float curtainDownDuration = 1.0f; // 幕が降りる時間
    [SerializeField] private Ease upEase = Ease.OutCubic; // 上昇時のイージング
    [SerializeField] private Ease downEase = Ease.InCubic; // 下降時のイージング

    private Canvas rootCanvas;
    private bool isOpened = false; // 幕が開いているかどうかの状態フラグ

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 幕の描画順（レイヤー）を動的に変更する
    /// </summary>
    public void SetSortingOrder(int order)
    {
        if (rootCanvas != null)
        {
            rootCanvas.sortingOrder = order;
        }
    }

    /// <summary>
    /// 幕が上がるアニメーション（予備動作あり）
    /// </summary>
    public async UniTask OpenCurtainAsync()
    {
        if (mainCurtain == null || lowerClosedTarget == null || lowerOpenTarget == null)
        {
            Debug.LogWarning("大幕またはターゲットポジションが設定されていません");
            return;
        }

        // すでに開いているなら処理をスキップして完了扱いにする
        if (isOpened)
        {
            SetCurtainVisibility(false);
            return;
        }

        Sequence sequence = DOTween.Sequence();

        // 少し下に動く（予備動作）
        _ = sequence.Append(
            mainCurtain.DOAnchorPosY(lowerClosedTarget.anchoredPosition.y - anticipationDistance, anticipationDuration)
                .SetEase(Ease.OutQuad)
        );

        // 大幕が指定の開位置に上がる
        _ = sequence.Append(
            mainCurtain.DOAnchorPos(lowerOpenTarget.anchoredPosition, mainCurtainUpDuration)
                .SetEase(upEase)
        );

        // 装飾カーテンが遅れて指定の開位置に上がる
        if (topDecoration != null && upperOpenTarget != null)
        {
            _ = sequence.Insert(
                anticipationDuration + mainCurtainUpDuration * topDecorationDelay / mainCurtainUpDuration,
                topDecoration.DOAnchorPos(upperOpenTarget.anchoredPosition, topDecorationUpDuration)
                    .SetEase(upEase)
            );
        }

        await sequence.ToUniTask();

        // アニメーション完了後、幕を非表示
        SetCurtainVisibility(false);
        isOpened = true; // 状態を開幕に更新
    }

    /// <summary>
    /// 幕が降りるアニメーション
    /// </summary>
    public async UniTask CloseCurtainAsync()
    {
        if (mainCurtain == null || lowerClosedTarget == null)
        {
            Debug.LogWarning("大幕またはターゲットポジションが設定されていません");
            return;
        }

        // すでに降りているなら処理をスキップして完了扱いにする
        if (!isOpened)
        {
            SetCurtainVisibility(true);
            return;
        }

        // 幕を表示してから降ろす
        SetCurtainVisibility(true);

        Sequence sequence = DOTween.Sequence();

        // 装飾カーテンから先に指定の閉位置に降りる
        if (topDecoration != null && upperClosedTarget != null)
        {
            _ = sequence.Append(
                topDecoration.DOAnchorPos(upperClosedTarget.anchoredPosition, curtainDownDuration * 0.6f)
                    .SetEase(downEase)
            );
        }

        // 大幕が指定の閉位置に降りる
        _ = sequence.Append(
            mainCurtain.DOAnchorPos(lowerClosedTarget.anchoredPosition, curtainDownDuration)
                .SetEase(downEase)
        );

        await sequence.ToUniTask();
        isOpened = false; // 状態を閉幕に更新
    }

    /// <summary>
    /// 幕の表示/非表示を切り替え
    /// </summary>
    private void SetCurtainVisibility(bool visible)
    {
        if (mainCurtain != null) mainCurtain.gameObject.SetActive(visible);
        if (topDecoration != null) topDecoration.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 幕を即座に閉じた状態にリセット
    /// </summary>
    public void ResetCurtainPosition()
    {
        if (mainCurtain != null && lowerClosedTarget != null)
        {
            mainCurtain.anchoredPosition = lowerClosedTarget.anchoredPosition;
        }
        if (topDecoration != null && upperClosedTarget != null)
        {
            topDecoration.anchoredPosition = upperClosedTarget.anchoredPosition;
        }

        SetCurtainVisibility(true);
        isOpened = false; // リセット時は閉幕状態にする
    }

    private void OnDestroy()
    {
        mainCurtain?.DOKill();
        topDecoration?.DOKill();
    }
}
