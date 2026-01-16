using System;
using System.Collections.Generic;

namespace UnityActorSystem {
    /// <summary>
    /// アクターステートマシン用のステート基底
    /// </summary>
    public abstract class ActorState<TKey, TBlackboard> : IActorState
        where TBlackboard : class, IActorStateBlackboard, new() {
        private ActorStateMachine<TKey, TBlackboard> _stateMachine;

        /// <summary>所有主アクター</summary>
        protected Actor<TKey> Owner => _stateMachine?.Owner;
        /// <summary>ブラックボード</summary>
        protected TBlackboard Blackboard => _stateMachine?.Blackboard;

        /// <inheritdoc/>
        void IActorState.Enter() => Enter();

        /// <inheritdoc/>
        void IActorState.Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) => Update(commands, signals, deltaTime);

        /// <inheritdoc/>
        void IActorState.Exit() => Exit();

        /// <summary>
        /// セットアップ処理
        /// </summary>
        /// <param name="stateMachine">所有主ステートマシン</param>
        internal void Setup(ActorStateMachine<TKey, TBlackboard> stateMachine) {
            _stateMachine = stateMachine;
            Setup();
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        protected virtual void Setup() { }

        /// <summary>
        /// 開始処理
        /// </summary>
        protected virtual void Enter() { }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="commands">処理対象のコマンドリスト</param>
        /// <param name="signals">処理対象のシグナルリスト</param>
        /// <param name="deltaTime">変位時間</param>
        protected virtual void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) { }

        /// <summary>
        /// 終了処理
        /// </summary>
        protected virtual void Exit() { }

        /// <summary>
        /// ステートの変更
        /// </summary>
        /// <param name="force">タイプに変化がなくても変更処理を行うか</param>
        protected void ChangeState<T>(bool force = false)
            where T : ActorState<TKey, TBlackboard> {
            if (_stateMachine == null) {
                throw new InvalidOperationException("State Machine is not set.");
            }

            _stateMachine.ChangeState(typeof(T), force);
        }
    }
}