using System;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ操作用クラス基底
    /// </summary>
    public abstract class CharacterController : IActorController {
        /// <summary>オーナーアクター</summary>
        protected Actor Owner { get; private set; }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface.Activate() { }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() { }

        /// <inheritdoc/>
        void IActorInterface.Attached(Actor actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
            Update(deltaTime);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        protected virtual void Update(float deltaTime) {
        }
    }
}