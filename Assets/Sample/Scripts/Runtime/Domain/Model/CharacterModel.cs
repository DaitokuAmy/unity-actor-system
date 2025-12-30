using System;
using UnityActorSystem;

namespace Sample.Domain {
    /// <summary>
    /// キャラ用のドメインモデル
    /// </summary>
    public abstract class CharacterModel : IActorModel {
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