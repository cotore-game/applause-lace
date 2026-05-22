using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 舞台幕の開閉と背景切り替えを制御するコントローラー
/// </summary>
public class CurtainController : MonoBehaviour
{
    [Header("幕の参照")]
    [SerializeField] private RectTransform mainCurtain;     // 大幕
    [SerializeField] private RectTransform topDecoration;   // 上部装飾カーテン

    [Header("アニメーション設定")]
    [SerializeField] private float anticipationDuration = 0.3f;     // 予備動作の時間
    [SerializeField] private float anticipationDistance = 50f;      // 予備動作の距離（下方向）
    [SerializeField] private float mainCurtainUpDuration = 1.0f;    // 大幕が上がる時間
    [SerializeField] private float topDecorationUpDuration = 0.8f;  // 装飾が上がる時間
    [SerializeField] private float topDecorationDelay = 0.5f;       // 装飾が動き始めるまでの遅延
    [SerializeField] private float curtainDownDuration = 1.0f;      // 幕が降りる時間
    [SerializeField] private Ease upEase = Ease.OutCubic;           // 上昇時のイージング
    [SerializeField] private Ease downEase = Ease.InCubic;          // 下降時のイージング

    private Vector2 mainCurtainInitialPos;
    private Vector2 topDecorationInitialPos;
    private RectTransform canvasRectTransform;
    private Canvas rootCanvas;

    private void Awake()
    {
        // 初期位置を保存
        if (mainCurtain != null)
        {
            mainCurtainInitialPos = mainCurtain.anchoredPosition;
        }
        if (topDecoration != null)
        {
            topDecorationInitialPos = topDecoration.anchoredPosition;
        }

        // Canvasの参照を取得
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            canvasRectTransform = rootCanvas.GetComponent<RectTransform>();
        }
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
        if (mainCurtain == null)
        {
            Debug.LogWarning("mainCurtainが設定されていません");
            return;
        }

        Sequence sequence = DOTween.Sequence();

        // 画面外上部の位置を計算
        float screenTop = GetScreenTopPosition();
        float mainCurtainHeight = mainCurtain.rect.height;
        float topDecorationHeight = topDecoration != null ? topDecoration.rect.height : 0;

        // 大幕の目標位置（画面外上部 + 幕の高さ分）
        float mainCurtainTargetY = screenTop + mainCurtainHeight;

        // 1. 予備動作：少し下に動く
        sequence.Append(
            mainCurtain.DOAnchorPosY(mainCurtainInitialPos.y - anticipationDistance, anticipationDuration)
                .SetEase(Ease.OutQuad)
        );

        // 2. 大幕が上に上がる
        sequence.Append(
            mainCurtain.DOAnchorPosY(mainCurtainTargetY, mainCurtainUpDuration)
                .SetEase(upEase)
        );

        // 3. 装飾カーテンが遅れて上がる
        if (topDecoration != null)
        {
            float topDecorationTargetY = screenTop + topDecorationHeight;

            sequence.Insert(
                anticipationDuration + mainCurtainUpDuration * topDecorationDelay / mainCurtainUpDuration,
                topDecoration.DOAnchorPosY(topDecorationTargetY, topDecorationUpDuration)
                    .SetEase(upEase)
            );
        }

        await sequence.ToUniTask();

        // アニメーション完了後、幕を非表示
        SetCurtainVisibility(false);
    }

    /// <summary>
    /// 幕が降りるアニメーション
    /// </summary>
    public async UniTask CloseCurtainAsync()
    {
        if (mainCurtain == null)
        {
            Debug.LogWarning("mainCurtainが設定されていません");
            return;
        }

        // 幕を表示してから降ろす
        SetCurtainVisibility(true);

        Sequence sequence = DOTween.Sequence();

        // 装飾カーテンから先に降りる
        if (topDecoration != null)
        {
            sequence.Append(
                topDecoration.DOAnchorPos(topDecorationInitialPos, curtainDownDuration * 0.6f)
                    .SetEase(downEase)
            );
        }

        // 大幕が降りる
        sequence.Append(
            mainCurtain.DOAnchorPos(mainCurtainInitialPos, curtainDownDuration)
                .SetEase(downEase)
        );

        await sequence.ToUniTask();
    }

    /// <summary>
    /// 幕の表示/非表示を切り替え
    /// </summary>
    /// <param name="visible">表示するかどうか</param>
    private void SetCurtainVisibility(bool visible)
    {
        if (mainCurtain != null)
        {
            mainCurtain.gameObject.SetActive(visible);
        }
        if (topDecoration != null)
        {
            topDecoration.gameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// 幕を即座に閉じた状態にリセット
    /// </summary>
    public void ResetCurtainPosition()
    {
        if (mainCurtain != null)
        {
            mainCurtain.anchoredPosition = mainCurtainInitialPos;
        }
        if (topDecoration != null)
        {
            topDecoration.anchoredPosition = topDecorationInitialPos;
        }

        // リセット時は幕を表示
        SetCurtainVisibility(true);
    }

    /// <summary>
    /// 画面上端の位置を取得（Anchor基準）
    /// </summary>
    private float GetScreenTopPosition()
    {
        if (canvasRectTransform == null) return 1000f;

        // Canvasの高さの半分が画面上端
        float canvasHeight = canvasRectTransform.rect.height;
        return canvasHeight / 2f;
    }

    private void OnDestroy()
    {
        // DOTweenのシーケンスをクリーンアップ
        mainCurtain?.DOKill();
        topDecoration?.DOKill();
    }
}
