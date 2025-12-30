using System;
using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクター本体
    /// </summary>
    public sealed class Actor : IActorRuntime {
        private readonly ActorStateMachine _stateMachine;
        private readonly ActorBuffer<ActorCommand> _commandBuffer = new();
        private readonly ActorBuffer<ActorSignal> _signalBuffer = new();
        private readonly List<ActorCommand> _workCommands = new();
        private readonly List<ActorSignal> _workSignals = new();

        private IActorController _controller;
        private IActorReceiver _receiver;
        private IActorModel _model;
        private IActorPresenter _presenter;
        private IActorView _view;
        private bool _active;
        private bool _disposed;

        /// <summary>アクティブ状態</summary>
        public bool IsActive => _active;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Actor() {
            _stateMachine = new ActorStateMachine(this);
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;

            ResetActorInterface(ref _view);
            ResetActorInterface(ref _presenter);
            ResetActorInterface(ref _model);
            ResetActorInterface(ref _receiver);
            ResetActorInterface(ref _controller);
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdatePreLogic(float deltaTime) {
            if (!_active) {
                return;
            }

            _controller?.Update(deltaTime);
            _receiver?.Update(deltaTime);
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdatePostLogic(float deltaTime) {
            if (!_active) {
                return;
            }

            _commandBuffer.GetBufferedContents(_workCommands);
            _signalBuffer.GetBufferedContents(_workSignals);
            _stateMachine.Update(_workCommands, _workSignals, deltaTime);
            _model?.Update(deltaTime);
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdateView(float deltaTime) {
            if (!_active) {
                return;
            }

            _presenter?.Update(deltaTime);
            _view?.Update(deltaTime);
        }

        /// <summary>
        /// Controllerの取得
        /// </summary>
        public T GetController<T>()
            where T : class, IActorController {
            return _controller as T;
        }

        /// <summary>
        /// Receiverの取得
        /// </summary>
        public T GetReceiver<T>()
            where T : class, IActorReceiver {
            return _receiver as T;
        }

        /// <summary>
        /// Presenterの取得
        /// </summary>
        public T GetPresenter<T>()
            where T : class, IActorPresenter {
            return _presenter as T;
        }

        /// <summary>
        /// Modelの取得
        /// </summary>
        public T GetModel<T>()
            where T : class, IActorModel {
            return _model as T;
        }

        /// <summary>
        /// Viewの取得
        /// </summary>
        public T GetView<T>()
            where T : class, IActorView {
            return _view as T;
        }

        /// <summary>
        /// アクティブ状態の設定
        /// </summary>
        public void SetActive(bool active) {
            if (_active == active || _disposed) {
                return;
            }

            _active = active;
            if (_active) {
                _controller?.Activate();
                _receiver?.Activate();
                _model?.Activate();
                _presenter?.Activate();
                _view?.Activate();
            }
            else {
                _view?.Deactivate();
                _presenter?.Deactivate();
                _model?.Deactivate();
                _receiver?.Deactivate();
                _controller?.Deactivate();
            }
        }

        /// <summary>
        /// StateMachineの初期化
        /// </summary>
        /// <param name="startStateType">開始ステートのタイプ</param>
        /// <param name="states">登録するステート一覧</param>
        public void SetupStates(Type startStateType, params ActorState[] states) {
            _stateMachine.SetStates(states);
            _stateMachine.ChangeState(startStateType);
        }

        /// <summary>
        /// Controllerの設定
        /// </summary>
        public void SetController(IActorController controller, bool prevDispose = true) {
            SetActorInterface(controller, ref _controller, prevDispose);
        }

        /// <summary>
        /// Receiverの設定
        /// </summary>
        public void SetReceiver(IActorReceiver receiver, bool prevDispose = true) {
            SetActorInterface(receiver, ref _receiver, prevDispose);
        }

        /// <summary>
        /// Modelの設定
        /// </summary>
        public void SetModel(IActorModel model, bool prevDispose = true) {
            SetActorInterface(model, ref _model, prevDispose);
        }

        /// <summary>
        /// Presenterの設定
        /// </summary>
        public void SetPresenter(IActorPresenter presenter, bool prevDispose = true) {
            SetActorInterface(presenter, ref _presenter, prevDispose);
        }

        /// <summary>
        /// Viewの設定
        /// </summary>
        public void SetView(IActorView view, bool prevDispose = true) {
            SetActorInterface(view, ref _view, prevDispose);
        }

        /// <summary>
        /// コマンドの生成
        /// </summary>
        public T CreateCommand<T>()
            where T : ActorCommand, new() {
            return _commandBuffer.CreateContent<T>();
        }

        /// <summary>
        /// シグナルの生成
        /// </summary>
        public T CreateSignal<T>()
            where T : ActorSignal, new() {
            return _signalBuffer.CreateContent<T>();
        }

        /// <summary>
        /// コマンドの追加
        /// </summary>
        /// <param name="command">追加対象のコマンド</param>
        public void AddCommand(ActorCommand command) {
            _commandBuffer.AddContent(command);
        }

        /// <summary>
        /// シグナルの追加
        /// </summary>
        /// <param name="signal">追加対象のシグナル</param>
        public void AddSignal(ActorSignal signal) {
            _signalBuffer.AddContent(signal);
        }

        /// <summary>
        /// IActorInterfaceの設定
        /// </summary>
        /// <param name="target">設定対象</param>
        /// <param name="field">設定先のフィールド</param>
        /// <param name="prevDispose">元々あった物をDisposeするか</param>
        private void SetActorInterface<T>(T target, ref T field, bool prevDispose = true)
            where T : class, IActorInterface {
            if (_disposed) {
                return;
            }

            ResetActorInterface(ref field, prevDispose);

            field = target;
            if (field != null) {
                field.Attached(this);
                if (_active) {
                    field.Activate();
                }
            }
        }

        /// <summary>
        /// IActorInterfaceの除外処理
        /// </summary>
        private void ResetActorInterface<T>(ref T field, bool dispose = true)
            where T : class, IActorInterface {
            if (field == null) {
                return;
            }

            if (_active) {
                field.Deactivate();
            }

            field.Detached();
            if (dispose) {
                field.Dispose();
            }

            field = null;
        }
    }
}