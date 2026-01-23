using UnityActorSystem;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラ用のブラックボード
    /// </summary>
    public class CharacterStateBlackboard : IActorStateBlackboard {
        /// <summary>移動向き情報</summary>
        public Vector2 MoveVector { get; set; }
        /// <summary>攻撃Index</summary>
        public int AttackIndex { get; set; }
        /// <summary>ノックバック方向</summary>
        public Vector3 KnockbackDirection { get; set; }
    }
}