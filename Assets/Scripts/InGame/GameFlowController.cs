using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
using AssetManagement;
using GameDialogue;

public class GameFlowController : IStartable
{
    private readonly ClapGameModel _model;
    private readonly GameView _view;
    private readonly StageData[] _stageList;
    private readonly DialogueSystem _tutorial;

    [Inject]
    public GameFlowController(
        ClapGameModel model,
        GameView view,
        StageData[] stageList,
        DialogueSystem tutorial)
    {
        _model = model;
        _view = view;
        _stageList = stageList;
        _tutorial = tutorial;
    }

    public void Start()
    {
        RunGameLoop().Forget();
    }

    private async UniTaskVoid RunGameLoop()
    {
        // チュートリアル
        await RunTutorialPhase();

        // 全てのステージを順番に
        foreach (var stageData in _stageList)
        {
            // ステージ実行
            await RunStagePhase(stageData);

            // ステージ間の暗転や遷移演出があればここに挟む
            await UniTask.Delay(1000);
        }

        // エンディング
        await RunEndingPhase();
    }

    private async UniTask RunTutorialPhase()
    {
        var tutorialData = ScriptableObject.CreateInstance<StageData>();
        tutorialData.Duration = 5f; // 固定

        await _tutorial.PlayDialogue("これからゲームを始めます。クリックで3回以上拍手してください。");

        await _view.ShowSpotlightOnButton(radius: 230f);

        // チュートリアルは特殊なので別途制御してもいいが、今回は簡易化
        _model.StartGame(tutorialData);
        await UniTask.WaitUntil(() => _model.Score >= 3);

        await _view.HideSpotlight();
        await _tutorial.PlayDialogue("いい感じです！では本番ですよ～？");
        _model.Stop();
    }

    // ステージ進行のコアロジック    
    private async UniTask RunStagePhase(StageData data)
    {
        _view.SetCharacter(data.CharacterImage);
        _model.StartGame(data);

        // 失敗か成功
        int resultType = await UniTask.WhenAny(
            WaitRoundFailed(),
            WaitClearCondition()
        );

        _model.Stop(); // ゲームループ停止

        if (resultType == 0) // 失敗
        {
            Debug.Log("Round Failed! Score -> 0");

            // _view.ShowAwkwardFace(); 
            await _tutorial.PlayDialogue("あらら…張り切りすぎですよ…");
        }
        else // 成功
        {
            Debug.Log($"Round Clear! Score: {_model.Score}");
            _view.ShowResult(_model.Score);
            await _tutorial.PlayDialogue("素晴らしい拍手でした！");
        }

        // 共通のリザルト表示時間
        await UniTask.Delay(2000);
    }

    private async UniTask RunEndingPhase()
    {
        // 最終的な合計スコアなどを計算してもよい
        await _tutorial.PlayDialogue("これでゲームは終わりです。");
        await _tutorial.PlayDialogue("私の働きも悪くなかったでしょう？");

        // エンディング中のクリック遊び用データ
        var endingData = ScriptableObject.CreateInstance<StageData>();
        endingData.Duration = 9999f;

        _model.StartGame(endingData);
        await _tutorial.PlayDialogue("褒めてくれてもいいんですよ？", waitForInput: true);

        int finalClicks = _model.Score;
        _model.Stop();

        if (finalClicks > 10)
            await _tutorial.PlayDialogue("…ありがとうございます（照）");
        else
            await _tutorial.PlayDialogue("冗談ですよ。また来てくださいね。");
    }

    #region HelperTasks

    // タイムアップ後のクリック
    private async UniTask WaitRoundFailed()
    {
        var source = new UniTaskCompletionSource();
        // イベントが来たらTask完了とする
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

    // タイムアップしてから、クリックせずに数秒耐える
    private async UniTask WaitClearCondition()
    {
        var source = new UniTaskCompletionSource();

        // 内部タイマーがリミットを超えたら発火
        Action timeUpHandler = () => {
            // ここからさらに2秒(演出余韻)クリックしなければクリア確定とみなす
            // この2秒の間にクリックしたら WaitRoundFailed の方が先に反応してキャンセルされる想定
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

    #endregion
}
