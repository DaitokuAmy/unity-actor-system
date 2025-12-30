using System;
using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクターステートマシン
    /// </summary>
    public sealed class ActorStateMachine : IActorInterface {
        private readonly Dictionary<Type, ActorState> _stateMap = new();

        private bool _disposed;
        private bool _activated;
        private bool _attached;
        private Type _currentStateType;
        private IActorState _currentState;
        
        /// <summary>現在のステート</summary>
        public ActorState CurrentState => (ActorState)_currentState;
        /// <summary>有効状態か</summary>
        private bool IsActive => !_disposed && _activated && _attached;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="states">保持するステートリスト</param>
        public ActorStateMachine(params ActorState[] states) {
            foreach (var state in states) {
                var key = state.GetType();
                _stateMap.TryAdd(key, state);
            }
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            if (_disposed) {
                return;
            }
            
            _stateMap.Clear();
            _disposed = true;
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
            if (_activated) {
                return;
            }
            
            _activated = true;
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
            if (!_activated) {
                return;
            }
            
            _activated = false;
        }

        /// <inheritdoc/>
        void IActorInterface.Attached(Actor actor) {
            if (_attached) {
                return;
            }
            
            _attached = true;
        }

        /// <inheritdoc/>
        void IActorInterface.Detached() {
            if (!_attached) {
                return;
            }
            
            ResetState();
            _attached = false;
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
            if (_currentState != null) {
                _currentState.Update(deltaTime);
            }
        }

        /// <summary>
        /// ステートの変更
        /// </summary>
        public void ChangeState<T>(bool force = false)
            where T : ActorState {
            if (!IsActive) {
                return;
            }
            
            var type = typeof(T);
            if (_currentStateType == type && !force) {
                return;
            }

            // ステートチェック
            if (!_stateMap.TryGetValue(type, out var nextState)) {
                throw new ArgumentException($"State type is not found. type={type}");
            }

            // 終了処理
            if (_currentState != null) {
                var state = _currentState;
                _currentState = null;
                _currentStateType = null;
                state.Exit();
            }

            // 開始処理
            _currentState = nextState;
            _currentStateType = type;
            _currentState.Enter();
        }

        /// <summary>
        /// カレントステートが無い状態にリセットする
        /// </summary>
        public void ResetState() {
            // 終了処理
            if (_currentState != null) {
                var state = _currentState;
                _currentState = null;
                _currentStateType = null;
                state.Exit();
            }
        }

        /// <summary>
        /// ステートの取得
        /// </summary>
        public T GetState<T>() where T : ActorState {
            if (!_stateMap.TryGetValue(typeof(T), out var state)) {
                return null;
            }

            return (T)state;
        }
    }
}