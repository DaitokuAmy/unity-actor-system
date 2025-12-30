namespace Sample {
    /// <summary>
    /// ActorBufferに詰め込むクラス用のインターフェース
    /// </summary>
    public interface IActorBufferContent {
        /// <summary>
        /// 生成時の処理
        /// </summary>
        void OnCreated();
        
        /// <summary>
        /// Pool返却時の処理
        /// </summary>
        void OnReleased();
    }
}