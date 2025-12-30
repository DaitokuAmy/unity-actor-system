using System;
using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクター本体
    /// </summary>
    public sealed class Actor : IActorRuntime {
        private readonly ActorStateMachine _stateMachine = new();
        private readonly ActorBuffer<ActorCommand> _commandBuffer = new();
        private readonly ActorBuffer<ActorSignal> _signalBuffer = new();
        private readonly List<ActorCommand> _workCommands = new();
        private readonly List<ActorSignal> _workSignals = new();
        
        private IActorController _controller;
        private IActorPresenter _presenter;
        private IActorReceiver _receiver;
        private IActorModel _model;
        private IActorView _view;
        
        /// <inheritdoc/>
        void IDisposable.Dispose() {
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdatePreLogic(float deltaTime) {
            _controller?.Update(deltaTime);
            _receiver?.Update(deltaTime);
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdatePostLogic(float deltaTime) {
            _commandBuffer.GetBufferedContents(_workCommands);
            _signalBuffer.GetBufferedContents(_workSignals);
            _stateMachine.Update(_workCommands, _workSignals, deltaTime);
            _model?.Update(deltaTime);
        }

        /// <inheritdoc/>
        void IActorRuntime.UpdateView(float deltaTime) {
            _presenter?.Update(deltaTime);
            _view?.Update(deltaTime);
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
    }
}