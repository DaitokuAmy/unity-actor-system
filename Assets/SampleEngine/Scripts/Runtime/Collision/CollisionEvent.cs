using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// 衝突イベント情報
    /// </summary>
    public readonly struct CollisionEvent {
        /// <summary>Hit側のId</summary>
        public readonly int hitId;
        /// <summary>受け側のId</summary>
        public readonly int receiveId;
        /// <summary>衝突位置</summary>
        public readonly Vector3 contactPoint;
        /// <summary>衝突向き</summary>
        public readonly Vector3 contactNormal;

        public CollisionEvent(int hitId, int receiveId, Vector3 contactPoint, Vector3 contactNormal) {
            this.hitId = hitId;
            this.receiveId = receiveId;
            this.contactPoint = contactPoint;
            this.contactNormal = contactNormal;
        }
    }
}