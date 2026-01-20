using System;
using Sample.Core;
using UnityActorSystem;

namespace Sample.Domain {
    /// <summary>
    /// カメラ用のドメインモデル
    /// </summary>
    public sealed class CameraModel : IActorModel<int>, IReadOnlyCameraModel {
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