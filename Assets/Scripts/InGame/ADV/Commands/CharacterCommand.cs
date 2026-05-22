using Cysharp.Threading.Tasks;
using UnityEngine;
using ADV.Core;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using System;
using System.Collections.Generic;

namespace ADV.Commands
{
    /// <summary>
    /// キャラクター表示コマンド
    /// Arg1: キャラ名 (Graduate, Host, Pianist, Scientist)
    /// Arg2: 表情名 (ファイル名)
    /// Arg3: イージング使用 (0=即座, 1=イージング, 省略時=1)
    /// </summary>
    public class CharacterCommand : CommandBase
    {
        private readonly CharacterPresenter _characterPresenter;

        // 非待機型コマンド（演出系）
        public override bool ShouldEngineAwait => false;

        /// <summary>
        /// コンストラクタインジェクション
        /// </summary>
        public CharacterCommand(CharacterPresenter characterPresenter)
        {
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
        }

        public override async UniTask ExecuteAsync(LineData<ScenarioFields> lineData, CancellableTask cancellable)
        {
            // パラメータ取得
            string characterName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);
            string expression = lineData.GetOrDefault<string>(ScenarioFields.Arg2, null);
            int useEasingInt = lineData.GetOrDefault<int>(ScenarioFields.Arg3, 1);
            bool useEasing = useEasingInt != 0;

            if (string.IsNullOrWhiteSpace(characterName) || string.IsNullOrWhiteSpace(expression))
            {
                Debug.LogWarning("[CharacterCommand] Character name or expression is empty");
                return;
            }

            // CancellableTaskからCancellationTokenを取得
            await _characterPresenter.ShowCharacter(
                characterName,
                expression,
                useEasing,
                cancellable.Token
            );
        }

        public override bool Validate(LineData<ScenarioFields> lineData, out string errorMessage)
        {
            errorMessage = null;

            var characterName = lineData.GetOrDefault<string>(ScenarioFields.Arg1, null);
            if (string.IsNullOrWhiteSpace(characterName))
            {
                errorMessage = "Character command requires character name in Arg1";
                return false;
            }

            var expression = lineData.GetOrDefault<string>(ScenarioFields.Arg2, null);
            if (string.IsNullOrWhiteSpace(expression))
            {
                errorMessage = "Character command requires expression in Arg2";
                return false;
            }

            return true;
        }
    }
}
