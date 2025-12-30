using System;
using UnityActorSystem;
using Sample.Core;

namespace Sample.Domain {
    /// <summary>
    /// プレイヤー用のドメインモデル
    /// </summary>
    public class PlayerModel : IActorModel, IReadOnlyPlayerModel {
        /// <inheritdoc/>
        public int AttackComboMax => 1;
        
        /// <inheritdoc/>
        void IDisposable.Dispose() {
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Attached(Actor actor) {
        }

        /// <inheritdoc/>
        void IActorInterface.Detached() {
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
        }
    }
}