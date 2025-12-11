using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using AssetManagement;
using GameDialogue;
using ADV.Presentation;

public class GamePlayController : IStartable
{
    private readonly ClapGameModel _model;
    private readonly GameView _view;
    private readonly StageData[] _stageList;
    private readonly TextPresenter _textPresenter;
    private readonly CharacterPresenter _characterPresenter;
    private readonly TitleView _titleView;
    private readonly ResultView _resultView;
    private readonly CountdownView _countdownView;

    [Inject]
    public GamePlayController(
        ClapGameModel model,
        GameView view,
        StageData[] stageList,
        TextPresenter textPresenter,
        CharacterPresenter characterPresenter,
        TitleView titleView,
        ResultView resultView,
        CountdownView countdownView)
    {
        _model = model;
        _view = view;
        _stageList = stageList;
        _textPresenter = textPresenter;
        _characterPresenter = characterPresenter;
        _titleView = titleView;
        _resultView = resultView;
        _countdownView = countdownView;
    }

    public void Start()
    {
        RunGameLoop().Forget();
    }

    private async UniTaskVoid RunGameLoop()
    {
        // ======================
        // タイトル画面
        // ======================
        await ShowTitlePhase();

        // ======================
        // チュートリアル
        // ======================
        await RunTutorialPhase();

        // ======================
        // ゲームラウンド1
        // ======================
        if (_stageList != null && _stageList.Length > 0)
        {
            await RunGameRound1(_stageList[0]);
        }

        // ======================
        // タイトルへ戻る
        // ======================
        await ReturnToTitle();
    }

    // ======================
    // タイトルフェーズ
    // ======================
    private async UniTask ShowTitlePhase()
    {
        Debug.Log("[GamePlay] タイトルフェーズ開始");

        // 初期状態セット
        _view.gameObject.SetActive(false); // ゲームUIを非表示
        _textPresenter.Hide(); // ADV非表示
        await _characterPresenter.HideCharacter(useEasing: false); // キャラ非表示

        // 幕を閉じた状態にリセット
        _view.ResetCurtain();

        // タイトル表示
        _titleView.Show();

        // Press Any Key 待機
        await _titleView.WaitForAnyKey();

        // タイトルをフェードアウト
        await _titleView.Hide();

        Debug.Log("[GamePlay] タイトルフェーズ終了");
    }

    // ======================
    // チュートリアルフェーズ
    // ======================
    private async UniTask RunTutorialPhase()
    {
        Debug.Log("[GamePlay] チュートリアルフェーズ開始");

        // ゲームUIをアクティブ化（ボタンは非表示のまま）
        _view.gameObject.SetActive(true);
        _view.HideClapButton();

        // スポットライトのマスクを非表示（暗転なし）
        _view.DisableSpotlightMask();

        // ADV TextDisplayView のみをイージングイン
        await _textPresenter.EaseInWindowAsync();

        // シナリオ再生
        await _textPresenter.DisplayTextAsync("司会者", "これからゲームを始めます。");
        await UniTask.Delay(500);

        // スポットライト点灯（ボタン位置）
        await _view.ShowSpotlightOnButton(radius: 230f);

        // キャラクターがイージングイン
        await _characterPresenter.ShowCharacter("Scientist", "scientist_smile");

        // シナリオ進める
        await _textPresenter.DisplayTextAsync("科学者", "クリックで3回以上拍手してください。");

        // ボタンをイージングイン表示
        await _view.ShowClapButton();

        // チュートリアル用のゲームデータ
        var tutorialData = ScriptableObject.CreateInstance<StageData>();
        tutorialData.Duration = 99999f; // 実質無制限

        _model.StartGame(tutorialData);

        // 3回以上クリックされるまで待機
        await UniTask.WaitUntil(() => _model.Score >= 3);

        _model.Stop();

        // スポットライト消灯
        await _view.HideSpotlight();

        // チュートリアル完了メッセージ
        await _textPresenter.DisplayTextAsync("科学者", "いい感じです！では本番ですよ～？");

        // キャラクター退場
        await _characterPresenter.HideCharacter();

        // ADVイージングアウト
        await _textPresenter.EaseOutWindowAsync();

        Debug.Log("[GamePlay] チュートリアルフェーズ終了");
    }

    // ======================
    // ゲームラウンド1
    // ======================
    private async UniTask RunGameRound1(StageData stageData)
    {
        Debug.Log("[GamePlay] ゲームラウンド1開始");

        // ADVをイージングアウト（念のため）
        await _textPresenter.EaseOutWindowAsync();

        // 背景、立ち絵セット
        _view.SetBackground(0); // 背景インデックス0
        await _characterPresenter.ShowCharacter("Graduate", "graduate_normal", useEasing: false);

        // カウントUIをリセット＆表示
        _countdownView.ResetCount();
        _countdownView.Show();

        // 幕と看板を同時に上げるアニメーション
        await _view.OpenCurtainAsync();

        // カウントダウン画像のカウントダウン表示 (3, 2, 1, Start!)
        await _countdownView.PlayCountdown();

        // ゲーム開始
        _model.StartGame(stageData);

        // 失敗か成功かを待機
        int resultType = await UniTask.WhenAny(
            WaitRoundFailed(),
            WaitClearCondition()
        );

        _model.Stop();

        // 終わり後、祝福終了アニメーション
        await PlayCelebrationAnimation();

        // SD立ち絵フェードアウト
        await _characterPresenter.HideCharacter();

        // スポットライトオーバーレイで少し暗くする
        await _view.DimWithSpotlight();

        // ADVイージングイン
        await _textPresenter.EaseInWindowAsync();

        // ResultTextDisplayViewを表示
        _resultView.Show();

        // リザルトの表示
        if (resultType == 0) // 失敗
        {
            Debug.Log("[GamePlay] ラウンド失敗");
            _resultView.ShowResult(0, "残念...");
            await _textPresenter.DisplayTextAsync("司会者", "あらら…張り切りすぎですよ…");
        }
        else // 成功
        {
            Debug.Log($"[GamePlay] ラウンド成功 - スコア: {_model.Score}");
            _resultView.ShowResult(_model.Score, "素晴らしい！");
            await _textPresenter.DisplayTextAsync("司会者", $"素晴らしい拍手でした！スコア: {_model.Score}");
        }

        await UniTask.Delay(2000);

        // ResultView非表示
        _resultView.Hide();

        // ADVイージングアウト
        await _textPresenter.EaseOutWindowAsync();

        // スポットライト解除
        await _view.ClearSpotlight();

        // 幕おろすアニメーション
        await _view.CloseCurtainAsync();

        Debug.Log("[GamePlay] ゲームラウンド1終了");
    }

    // ======================
    // タイトルへ戻る
    // ======================
    private async UniTask ReturnToTitle()
    {
        Debug.Log("[GamePlay] タイトルへ戻ります");

        // 少し待機
        await UniTask.Delay(1000);

        // ゲームUIを非表示
        _view.gameObject.SetActive(false);

        // タイトル画面へ
        await ShowTitlePhase();
    }

    // ======================
    // ヘルパーメソッド
    // ======================

    /// <summary>
    /// 祝福アニメーション（イージングイン、イージングアウト）
    /// </summary>
    private async UniTask PlayCelebrationAnimation()
    {
        // 紙吹雪などのエフェクトを再生
        _view.PlayHappyEffect();
        await UniTask.Delay(1500);
        _view.StopClapEffects();
    }

    /// <summary>
    /// タイムアップ後のクリックで失敗
    /// </summary>
    private async UniTask WaitRoundFailed()
    {
        var source = new UniTaskCompletionSource();
        Action handler = () => source.TrySetResult();

        _model.OnRoundFailed += handler;
        try
        {
            await source.Task;
        }
        finally
        {
            _model.OnRoundFailed -= handler;
        }
    }

    /// <summary>
    /// タイムアップ後、2秒クリックしなければクリア
    /// </summary>
    private async UniTask WaitClearCondition()
    {
        var source = new UniTaskCompletionSource();

        Action timeUpHandler = () =>
        {
            // タイムアップから2秒後にクリア確定
            UniTask.Delay(2000)
                .ContinueWith(() => source.TrySetResult()).Forget();
        };

        _model.OnHiddenTimeUp += timeUpHandler;
        try
        {
            await source.Task;
        }
        finally
        {
            _model.OnHiddenTimeUp -= timeUpHandler;
        }
    }
}
