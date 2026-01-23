using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラのダメージ処理に関するInputPort
    /// </summary>
    public interface ICharacterDamageInputPort {
        /// <summary>
        /// 攻撃のヒット
        /// </summary>
        void HitAttack(int attackActorId, int receiveActorId, Vector3 contactPoint, Vector3 contactNormal, AttackParams attackParams);
    }
}