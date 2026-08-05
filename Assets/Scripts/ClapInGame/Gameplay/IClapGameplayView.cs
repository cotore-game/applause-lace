using System;

/// <summary>
/// 拍手ラウンド中の入力と表示をPresenterへ公開します。
/// ステージ全体の演出操作は含みません。
/// </summary>
public interface IClapGameplayView
{
    /// <summary>プレイヤーが有効な押下と解放を完了したときに通知されます。</summary>
    event Action OnClapButtonClicked;

    /// <summary>拍手UIを初期化して入力受付を開始します。</summary>
    void BeginGameplay();

    /// <summary>入力受付を終了して拍手UIを隠します。</summary>
    void EndGameplay();

    /// <summary>現在の拍手回数を表示します。</summary>
    void UpdateClapCount(int count);

    /// <summary>成立した拍手1回分の視覚効果を再生します。</summary>
    void PlayClapEffect();
}
