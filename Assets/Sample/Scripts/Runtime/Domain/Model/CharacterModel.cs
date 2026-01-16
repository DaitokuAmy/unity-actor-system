using System;
using UnityActorSystem;

namespace Sample.Domain {
    /// <summary>
    /// キャラ用のドメインモデル
    /// </summary>
    public abstract class CharacterModel : IActorModel<int> {
        /// <inheritdoc/>
        void IDisposable.Dispose() {
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) {
        }
    }
}