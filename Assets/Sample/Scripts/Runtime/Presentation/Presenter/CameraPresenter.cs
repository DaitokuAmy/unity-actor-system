using System;
using Sample.Application;
using Sample.Core;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// カメラ見た目反映用クラス
    /// </summary>
    public class CameraPresenter : ICameraPresenter, IActorPresenter<int> {
        private IAimTarget _currentTarget;

        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CameraActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraPresenter(CameraActorView actorView) {
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() { }

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
        void IActorInterface<int>.Update(float deltaTime) {
            // Targetの反映
            if (_currentTarget != null) {
                ActorView.SetTargetTransform(_currentTarget.Position, _currentTarget.Rotation);
            }
        }

        /// <inheritdoc/>
        void ICameraPresenter.ChangeTarget(IAimTarget aimTarget) {
            _currentTarget = aimTarget;
        }
    }
}