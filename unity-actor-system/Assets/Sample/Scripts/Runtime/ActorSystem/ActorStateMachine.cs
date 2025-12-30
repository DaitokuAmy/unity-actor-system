using System;
using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクターステートマシン
    /// </summary>
    public sealed class ActorStateMachine : IDisposable {
        private readonly Actor _owner;
        private readonly Dictionary<Type, ActorState> _stateMap = new();

        private bool _disposed;
        private Type _currentStateType;
        private IActorState _currentState;

        /// <summary>現在のステート</summary>
        public ActorState CurrentState => (ActorState)_currentState;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">所有主</param>
        public ActorStateMachine(Actor owner) {
            _owner = owner;
        }

        /// <summary>
        /// ステートの設定
        /// </summary>
        /// <param name="states">保持するステートリスト</param>
        public void SetStates(params ActorState[] states) {
            ResetState();
            
            _stateMap.Clear();
            foreach (var state in states) {
                var key = state.GetType();
                if (_stateMap.TryAdd(key, state)) {
                    ((IActorState)state).Setup(_owner);
                }
            }
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            ResetState();
            _stateMap.Clear();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="commands">処理対象のコマンドリスト</param>
        /// <param name="signals">処理対象のシグナルリスト</param>
        /// <param name="deltaTime">変位時間</param>
        public void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
            if (_currentState != null) {
                _currentState.Update(commands, signals, deltaTime);
            }
        }

        /// <summary>
        /// ステートの変更
        /// </summary>
        public void ChangeState(Type type, bool force = false) {
            if (_disposed) {
                return;
            }

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