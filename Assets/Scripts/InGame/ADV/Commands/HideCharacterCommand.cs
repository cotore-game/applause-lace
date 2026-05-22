using Cysharp.Threading.Tasks;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using System;

namespace ADV.Commands
{
    /// <summary>
    /// キャラクターを非表示にさせるコマンド
    /// Arg1: イージング使用 (0=即座, 1=イージング, 省略時=1)
    /// </summary>
    public class HideCharacterCommand : CommandBase
    {
        private readonly CharacterPresenter _characterPresenter;

        // 待機型コマンド
        public override bool ShouldEngineAwait => true;

        /// <summary>
        /// コンストラクタインジェクション
        /// </summary>
        public HideCharacterCommand(CharacterPresenter characterPresenter)
        {
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            int useEasingInt = lineData.GetOrDefault<int>(ScenarioFields.Arg1, 1);
            bool useEasing = useEasingInt != 0;

            await _characterPresenter.HideCharacter(useEasing, cancellable.Token);
        }

        public override bool Validate(LineData<ScenarioFields> lineData, out string errorMessage)
        {
            // このコマンドに必須引数はなし
            errorMessage = null;
            return true;
        }
    }
}
