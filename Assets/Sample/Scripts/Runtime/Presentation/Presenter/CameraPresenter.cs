using System;
using Sample.Application;
using Sample.Core;
using UnityActorSystem;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// カメラ見た目反映用クラス
    /// </summary>
    public class CameraPresenter : ICameraPresenter, IActorPresenter<int> {
        private IActorTransform _baseTargetTransform;

        /// <summary>オーナーアクター</summary>
        private Actor<int> Owner { get; set; }
        /// <summary>参照用のモデル</summary>
        private IReadOnlyCameraModel Model { get; }
        /// <summary>制御用のビュー</summary>
        private CameraActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraPresenter(IReadOnlyCameraModel model, CameraActorView actorView) {
            Model = model;
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
            // カメラ情報の反映
            if (_baseTargetTransform != null) {
                var position = _baseTargetTransform.Position;
                var angleX = Model.AngleX;
                var angleY = Model.AngleY;
                var rotation = Quaternion.Euler(angleX, angleY, 0.0f);
                ActorView.SetTargetTransform(position, rotation);
            }
        }

        /// <inheritdoc/>
        void ICameraPresenter.ChangeBaseTarget(IActorTransform targetTransform) {
            _baseTargetTransform = targetTransform;
        }
    }
}