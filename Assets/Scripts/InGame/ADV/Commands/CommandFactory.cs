using UnityEngine;
using System.Collections.Generic;
using System;
using ADV.Presentation;
using SceneManagement;

namespace ADV.Commands
{
    /// <summary>
    /// コマンドインスタンスを生成し、必要な依存関係を注入するファクトリークラス
    /// </summary>
    public class CommandFactory
    {
        // コマンド登録用デリゲート
        private readonly Dictionary<string, Func<CommandBase>> _commandRegistry;

        // 各Presenterへの参照
        private readonly TextPresenter _textPresenter;
        private readonly CharacterPresenter _characterPresenter;
        private readonly SceneTransitioner _sceneTransitioner;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommandFactory(
            TextPresenter textPresenter,
            CharacterPresenter characterPresenter,
            SceneTransitioner sceneTransitioner)
        {
            _textPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
            _sceneTransitioner = sceneTransitioner ?? throw new ArgumentNullException(nameof(sceneTransitioner));

            _commandRegistry = new Dictionary<string, Func<CommandBase>>(StringComparer.OrdinalIgnoreCase);

            RegisterDefaultCommands();
        }

        /// <summary>
        /// デフォルトコマンドの登録
        /// </summary>
        private void RegisterDefaultCommands()
        {
            // テキスト
            Register("Text", () => new TextCommand(_textPresenter));

            // キャラクター表示系
            Register("Character", () => new CharacterCommand(_characterPresenter));
            Register("HideCharacter", () => new HideCharacterCommand(_characterPresenter));

            // シーン遷移系
            Register("LoadScene", () => new LoadSceneCommand(_sceneTransitioner));
            Register("End", () => new EndCommand());

            /*
            // 背景・演出系
            Register("Bg", () => new BgCommand());
            Register("Day", () => new DayCommand());
            Register("DayOff", () => new DayOffCommand());

            // 音声系
            Register("Bgm", () => new BgmCommand());
            Register("Se", () => new SeCommand());
            Register("StopBgm", () => new StopBgmCommand());
            Register("PauseBgm", () => new PauseBgmCommand());
            Register("SetVol", () => new SetVolCommand());

            // 制御系
            Register("Wait", () => new WaitCommand());
            Register("Goto", () => new GotoCommand());
            Register("If", () => new IfCommand());
            Register("Flag", () => new FlagCommand());
            Register("Param", () => new ParamCommand());

            // ウィンドウ制御
            Register("HideWindow", () => new HideWindowCommand(_textPresenter));
            Register("ShowWindow", () => new ShowWindowCommand(_textPresenter));

            // 特殊制御
            Register("Await", () => new AwaitCommand());
            Register("Button", () => new ButtonCommand());
            */
        }

        /// <summary>
        /// カスタムコマンドを登録
        /// </summary>
        public void Register(string commandName, Func<CommandBase> factory)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                Debug.LogWarning("[CommandFactory] Cannot register command with empty name");
                return;
            }

            if (_commandRegistry.ContainsKey(commandName))
            {
                Debug.LogWarning($"[CommandFactory] Command '{commandName}' is already registered. Overwriting.");
            }

            _commandRegistry[commandName] = factory;
        }

        /// <summary>
        /// コマンドインスタンスを生成（必要な依存関係のみ注入）
        /// </summary>
        public CommandBase CreateCommandInstance(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                commandName = ""; // 空欄はデフォルトコマンド
            }

            if (_commandRegistry.TryGetValue(commandName, out var factory))
            {
                return factory();
            }

            Debug.LogError($"[CommandFactory] Command '{commandName}' not found");
            return null;
        }

        /// <summary>
        /// 登録されているコマンド一覧を取得
        /// </summary>
        public IReadOnlyCollection<string> GetRegisteredCommands()
        {
            return _commandRegistry.Keys;
        }
    }
}
