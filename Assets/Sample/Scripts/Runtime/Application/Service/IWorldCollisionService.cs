using SampleEngine;

namespace Sample.Application {
    /// <summary>
    /// カメラサービス
    /// </summary>
    public interface IWorldCollisionService {
        /// <summary>
        /// 更新
        /// </summary>
        void Update(float deltaTime);
        
        /// <summary>
        /// 受けコリジョンの登録
        /// </summary>
        /// <param name="actorId">通知に使うActorId</param>
        /// <param name="collider">コライダー情報</param>
        /// <param name="layerMask">レイヤーマスク</param>
        /// <param name="listener">衝突検出用リスナー</param>
        /// <returns>コリジョンId</returns>
        int RegisterReceive(int actorId, IReceiveCollider collider, int layerMask, IWorldCollisionListener listener);

        /// <summary>
        /// ヒットコリジョンの登録
        /// </summary>
        /// <param name="actorId">通知に使うActorId</param>
        /// <param name="collider">コライダー情報</param>
        /// <param name="layerMask">レイヤーマスク</param>
        /// <returns>コリジョンId</returns>
        int RegisterHit(int actorId, IHitCollider collider, int layerMask);

        /// <summary>
        /// 受けコリジョンの登録解除
        /// </summary>
        void UnregisterReceive(int collisionId);

        /// <summary>
        /// ヒットコリジョンの登録解除
        /// </summary>
        void UnregisterHit(int collisionId);
    }
}