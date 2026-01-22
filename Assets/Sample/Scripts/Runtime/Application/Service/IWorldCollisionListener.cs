using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// ワールドコリジョンの検出通知インターフェース
    /// </summary>
    public interface IWorldCollisionListener {
        /// <summary>
        /// 衝突開始
        /// </summary>
        void OnCollisionEnter(int hitActorId, int receiveActorId, Vector3 contactPoint);

        /// <summary>
        /// 衝突中
        /// </summary>
        void OnCollisionStay(int hitActorId, int receiveActorId, Vector3 contactPoint);

        /// <summary>
        /// 衝突終了
        /// </summary>
        void OnCollisionExit(int hitActorId, int receiveActorId);
    }
}