using UnityEngine;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// キャラサービス
    /// </summary>
    public sealed class CharacterService : ICharacterDamageInputPort {
        [Inject]
        private readonly CharacterManager _characterManager;

        /// <inheritdoc/>
        void ICharacterDamageInputPort.HitAttack(int attackActorId, int receiveActorId, Vector3 contactPoint, Vector3 contactNormal, AttackParams attackParams) {
            if (!_characterManager.TryGetActor(attackActorId, out var attacker)) {
                return;
            }

            if (!_characterManager.TryGetActor(receiveActorId, out var receiver)) {
                return;
            }
            
            // ダメージ通知
            var signal = receiver.CreateSignal<CharacterSignals.Hit>();
            signal.Set(attackActorId, contactPoint, contactNormal, attackParams);
            receiver.AddSignal(signal);
        }
    }
}