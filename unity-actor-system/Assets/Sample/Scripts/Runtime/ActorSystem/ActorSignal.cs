namespace Sample {
    /// <summary>
    /// アクター用のシグナル(イベント情報)基底
    /// </summary>
    public class ActorSignal : IActorBufferContent {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ActorSignal() {}
        
        /// <inheritdoc/>
        void IActorBufferContent.OnCreated() => OnCreated();

        /// <inheritdoc/>
        void IActorBufferContent.OnReleased() => OnReleased();
        
        /// <summary>
        /// 生成時処理
        /// </summary>
        protected virtual void OnCreated() {}
        
        /// <summary>
        /// プール返却時処理
        /// </summary>
        protected virtual void OnReleased() {}
    }
}