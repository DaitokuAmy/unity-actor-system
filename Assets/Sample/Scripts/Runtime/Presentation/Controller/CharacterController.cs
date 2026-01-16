using System;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ操作用クラス基底
    /// </summary>
    public abstract class CharacterController : IActorController<int> {
        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            Activate();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            Deactivate();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) {
            Update(deltaTime);
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        protected virtual void Activate() {}

        /// <summary>
        /// 非アクティブ時処理
        /// </summary>
        protected virtual void Deactivate() {}

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        protected virtual void Update(float deltaTime) {}
    }
}