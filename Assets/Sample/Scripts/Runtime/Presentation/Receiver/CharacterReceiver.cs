using System;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャライベント監視用クラス
    /// </summary>
    public class CharacterReceiver : IActorReceiver<int> {
        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CharacterActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterReceiver(CharacterActorView actorView) {
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            //Owner.AddSignal();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) { }
    }
}