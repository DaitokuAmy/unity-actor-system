namespace Sample {
    public interface IActorState {
        /// <summary>
        /// 開始処理
        /// </summary>
        void Enter();

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        void Update(float deltaTime);

        /// <summary>
        /// 終了処理
        /// </summary>
        void Exit();
    }

    /// <summary>
    /// アクターステートマシン用のステート基底
    /// </summary>
    public abstract class ActorState : IActorState {
        /// <inheritdoc/>
        void IActorState.Enter() => Enter();

        /// <inheritdoc/>
        void IActorState.Update(float deltaTime) => Update(deltaTime);

        /// <inheritdoc/>
        void IActorState.Exit() => Exit();

        /// <summary>
        /// 開始処理
        /// </summary>
        protected abstract void Enter();

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        protected abstract void Update(float deltaTime);

        /// <summary>
        /// 終了処理
        /// </summary>
        protected abstract void Exit();
    }
}