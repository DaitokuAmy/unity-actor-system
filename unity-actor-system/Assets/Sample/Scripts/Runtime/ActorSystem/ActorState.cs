using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクターステートマシン用のステート基底
    /// </summary>
    public abstract class ActorState : IActorState {
        /// <summary>所有主アクター</summary>
        protected Actor Owner { get; private set; }

        /// <inheritdoc/>
        void IActorState.Setup(Actor owner) {
            Owner = owner;
            Setup();
        }

        /// <inheritdoc/>
        void IActorState.Enter() => Enter();

        /// <inheritdoc/>
        void IActorState.Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) => Update(commands, signals, deltaTime);

        /// <inheritdoc/>
        void IActorState.Exit() => Exit();

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
    }
}