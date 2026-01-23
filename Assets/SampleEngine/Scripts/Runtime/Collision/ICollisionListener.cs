namespace SampleEngine {
    /// <summary>
    /// 衝突検知リスナー
    /// </summary>
    public interface ICollisionListener {
        /// <summary>
        /// 衝突開始
        /// </summary>
        void OnCollisionEnter(in CollisionEvent evt);

        /// <summary>
        /// 衝突中
        /// </summary>
        void OnCollisionStay(in CollisionEvent evt);

        /// <summary>
        /// 衝突終了
        /// </summary>
        void OnCollisionExit(in CollisionEvent evt);
    }
}