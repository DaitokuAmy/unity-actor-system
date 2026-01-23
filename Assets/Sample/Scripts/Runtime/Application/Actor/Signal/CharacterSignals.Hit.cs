using UnityActorSystem;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラ用シグナル定義クラス
    /// </summary>
    partial class CharacterSignals {
        /// <summary>
        /// ヒット通知
        /// </summary>
        public class Hit : ActorSignal {
            /// <inheritdoc/>
            protected override int Order => (int)SignalOrder.Hit;
            
            /// <summary>攻撃アクターのId</summary>
            public int AttackActorId { get; private set; }
            /// <summary>衝突位置</summary>
            public Vector3 ContactPoint { get; private set; }
            /// <summary>衝突向き</summary>
            public Vector3 ContactNormal { get; private set; }
            /// <summary>攻撃パラメータ</summary>
            public AttackParams AttackParams { get; private set; }
            
            /// <summary>
            /// 値の設定
            /// </summary>
            /// <param name="attackActorId">攻撃主のActorId</param>
            /// <param name="contactPoint">衝突位置</param>
            /// <param name="contactNormal">衝突向き</param>
            /// <param name="attackParams">攻撃パラメータ</param>
            public void Set(int attackActorId, Vector3 contactPoint, Vector3 contactNormal, AttackParams attackParams) {
                AttackActorId = attackActorId;
                ContactPoint = contactPoint;
                ContactNormal = contactNormal;
                AttackParams = attackParams;
            }
        }
    }
}