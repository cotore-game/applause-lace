using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using ADV.Commands;
using ADV.Presentation;
using CSV4Unity;
using CSV4Unity.Fields;
using SceneManagement;
using VContainer.Unity;
using ADV.Core;

namespace ADV.System
{
    /// <summary>
    /// ADVシナリオ実行を統括するメインエンジン
    /// </summary>
    public class AdvScenarioExecutor : IStartable, IDisposable
    {
        // 依存関係
        private readonly TextPresenter _textPresenter;
        private readonly CharacterPresenter _characterPresenter;
        private readonly CommandFactory _commandFactory;

        // 設定
        private readonly bool _enableDebugLog;
        private readonly TextAsset _defaultScenarioData;

        // 実行状態
        private CsvData<ScenarioFields> _currentScenario;
        private int _currentLineIndex;
        private CancellableTask _scenarioCancellable;

        // 非同期演出タスク管理
        private readonly List<UniTask> _activeVisualTasks = new();

        // 実行状態プロパティ
        public bool IsExecuting { get; private set; }
        public bool IsPaused { get; private set; }
        public int TotalLines => _currentScenario?.RowCount ?? 0;
        public int CurrentLine => _currentLineIndex;
        public float Progress => TotalLines > 0 ? (float)_currentLineIndex / TotalLines : 0f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AdvScenarioExecutor(
            TextPresenter textPresenter,
            CharacterPresenter characterPresenter,
            CommandFactory commandFactory,
            bool enableDebugLog,
            TextAsset defaultScenarioData)
        {
            _textPresenter = textPresenter ?? throw new ArgumentNullException(nameof(textPresenter));
            _characterPresenter = characterPresenter ?? throw new ArgumentNullException(nameof(characterPresenter));
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            _enableDebugLog = enableDebugLog;
            _defaultScenarioData = defaultScenarioData;

            DebugLog("AdvScenarioExecutor initialized with VContainer");
        }

        /// <summary>
        /// VContainer IStartable実装 - シーン開始時に自動実行
        /// </summary>
        void IStartable.Start()
        {
            InitializeFromSceneData().Forget();
        }

        /// <summary>
        /// シーン遷移データからシナリオを初期化して実行
        /// </summary>
        private async UniTaskVoid InitializeFromSceneData()
        {
            try
            {
                var sceneData = SceneExchangeManager.Instance?.GetData<IAdvSceneData>();

                // シーン遷移データがない、またはCSVが設定されていない場合
                if (sceneData?.DataFile == null)
                {
                    Debug.LogWarning("[AdvScenarioExecutor] CSV data not found in scene exchange data");

                    if (_defaultScenarioData != null)
                    {
                        Debug.LogWarning("Using default scenario data");
                        await LoadAndExecuteScenario(_defaultScenarioData);
                    }
                    else
                    {
                        Debug.LogError("[AdvScenarioExecutor] No scenario data available");
                    }
                    return;
                }

                // シーン遷移データからCSVを取得して実行
                await LoadAndExecuteScenario(sceneData.DataFile);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Failed to initialize: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// シナリオをロードして実行開始
        /// </summary>
        public async UniTask LoadAndExecuteScenario(TextAsset csvFile)
        {
            if (IsExecuting)
            {
                Debug.LogWarning("[AdvScenarioExecutor] Already executing scenario. Stopping current.");
                await StopScenario();
            }

            try
            {
                // CSVロード
                var options = new CsvLoaderOptions
                {
                    HasHeader = true,
                    TrimFields = true,
                    IgnoreEmptyLines = true,
                    CommentPrefix = "#",
                    ValidationEnabled = true,
                    ThrowOnValidationError = false
                };

                _currentScenario = CSVLoader.LoadCSV<ScenarioFields>(csvFile, options);
                _currentLineIndex = 0;

                DebugLog($"Loaded scenario: {csvFile.name} ({_currentScenario.RowCount} lines)");

                // シナリオ実行開始
                _scenarioCancellable = new CancellableTask();
                IsExecuting = true;

                await ExecuteScenarioLoop();
            }
            catch (OperationCanceledException)
            {
                DebugLog("Scenario execution cancelled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Scenario execution failed: {ex}");
            }
            finally
            {
                await CleanupScenario();
            }
        }

        /// <summary>
        /// メインシナリオ実行ループ
        /// </summary>
        private async UniTask ExecuteScenarioLoop()
        {
            while (_currentLineIndex < _currentScenario.RowCount)
            {
                if (_scenarioCancellable.IsCancellationRequested) break;

                // ポーズ中は待機
                await UniTask.WaitUntil(() => !IsPaused);

                var lineData = _currentScenario.Rows[_currentLineIndex];
                var commandName = lineData.Get<string>(ScenarioFields.Command);

                // コマンドインスタンスを取得
                var command = _commandFactory.CreateCommandInstance(commandName);
                if (command == null)
                {
                    Debug.LogWarning($"[AdvScenarioExecutor] Unknown command: {commandName} at line {_currentLineIndex + 1}. Skipping.");
                    _currentLineIndex++;
                    continue;
                }

                try
                {
                    // バッチ処理に対応しているか確認
                    if (command.CanBatchProcess)
                    {
                        // 連続する同じコマンドを収集
                        var batchLineData = CollectBatchCommands(_currentLineIndex, commandName);

                        DebugLog($"[{_currentLineIndex + 1}-{_currentLineIndex + batchLineData.Count}] Batch executing {batchLineData.Count} '{commandName}' commands");

                        // バッチ実行とawait判断をコマンド自身に委譲
                        bool shouldAwait = command.ShouldBatchAwait(batchLineData);
                        var task = command.ExecuteBatchAsync(batchLineData, _scenarioCancellable);

                        if (shouldAwait)
                        {
                            // 待機型コマンドと同様に、先に全演出完了を待つ
                            await WaitForAllVisualTasks();
                            // バッチの完了を待つ
                            await task;
                        }
                        else
                        {
                            // 非待機タスクとして登録
                            _activeVisualTasks.Add(task);
                        }

                        // バッチ処理した行数分、インデックスを進める
                        _currentLineIndex += batchLineData.Count;
                    }
                    else
                    {
                        // 通常の単一行コマンド実行 
                        DebugLog($"[{_currentLineIndex + 1}/{TotalLines}] Executing: {commandName}");

                        // 従来の ExecuteCommand ロジック
                        await ExecuteCommand(command, lineData);

                        _currentLineIndex++;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[AdvScenarioExecutor] Command '{commandName}' failed at line {_currentLineIndex + 1}: {ex.Message}\n{ex.StackTrace}");
                    // エラーが発生しても次の行に進む
                    _currentLineIndex++;
                }
            }

            DebugLog("Scenario execution completed");
        }

        /// <summary>
        /// 連続する同じコマンドの行データを収集
        /// </summary>
        private List<LineData<ScenarioFields>> CollectBatchCommands(int startIndex, string batchCommandName)
        {
            var batch = new List<LineData<ScenarioFields>>();
            int scanIndex = startIndex;

            while (scanIndex < _currentScenario.RowCount)
            {
                var lineData = _currentScenario.Rows[scanIndex];
                var commandName = lineData.Get<string>(ScenarioFields.Command);

                // 指定されたコマンド名と一致するか
                if (commandName.Equals(batchCommandName, StringComparison.OrdinalIgnoreCase))
                {
                    batch.Add(lineData);
                    scanIndex++;
                }
                else
                {
                    break;
                }
            }

            return batch;
        }

        /// <summary>
        /// コマンド実行の中核ロジック
        /// </summary>
        private async UniTask ExecuteCommand(CommandBase command, LineData<ScenarioFields> lineData)
        {
            // コマンド検証
            if (!command.Validate(lineData, out var errorMsg))
            {
                Debug.LogError($"[AdvScenarioExecutor] Validation failed: {errorMsg}");
                return;
            }

            // 待機が必要なコマンドの場合、先に全演出完了を待つ
            if (command.ShouldEngineAwait)
            {
                await WaitForAllVisualTasks();

                // 待機型コマンドを実行
                await command.ExecuteAsync(lineData, _scenarioCancellable);
            }
            else
            {
                // タスクをリストに追加して即座に次へ
                var task = command.ExecuteAsync(lineData, _scenarioCancellable);
                _activeVisualTasks.Add(task);
            }
        }

        /// <summary>
        /// 全ての実行中演出タスクの完了を待機
        /// </summary>
        private async UniTask WaitForAllVisualTasks()
        {
            if (_activeVisualTasks.Count == 0) return;

            try
            {
                await UniTask.WhenAll(_activeVisualTasks).AttachCancellation(_scenarioCancellable);
            }
            catch (OperationCanceledException)
            {
                // キャンセルは正常
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AdvScenarioExecutor] Visual task error: {ex.Message}");
            }
            finally
            {
                _activeVisualTasks.Clear();
            }
        }

        /// <summary>
        /// シナリオ実行を停止
        /// </summary>
        public async UniTask StopScenario()
        {
            if (!IsExecuting) return;

            DebugLog("Stopping scenario execution");

            _scenarioCancellable?.Cancel();
            await CleanupScenario();
        }

        /// <summary>
        /// リソースクリーンアップ
        /// </summary>
        private async UniTask CleanupScenario()
        {
            IsExecuting = false;
            _activeVisualTasks.Clear();

            // キャッシュクリア
            _characterPresenter?.ClearCache();

            await UniTask.Yield();
        }

        /// <summary>
        /// ポーズ切り替え
        /// </summary>
        public void TogglePause()
        {
            IsPaused = !IsPaused;
            DebugLog($"Scenario {(IsPaused ? "paused" : "resumed")}");
        }

        /// <summary>
        /// 指定行へジャンプ
        /// </summary>
        public void JumpToLine(int lineIndex)
        {
            if (lineIndex < 0 || lineIndex >= TotalLines)
            {
                Debug.LogWarning($"[AdvScenarioExecutor] Invalid line index: {lineIndex}");
                return;
            }

            _currentLineIndex = lineIndex;
            DebugLog($"Jumped to line {lineIndex}");
        }

        private void DebugLog(string message)
        {
            if (_enableDebugLog)
            {
                Debug.Log($"<color=cyan>[AdvScenarioExecutor]</color> {message}");
            }
        }

        public void Dispose()
        {
            _scenarioCancellable?.Dispose();
            _textPresenter?.Dispose();
            _characterPresenter?.Dispose();
        }
    }
}
