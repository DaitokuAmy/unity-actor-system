using System;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ制御用ビュー
    /// </summary>
    public class CameraActorView : IActorView {
        private readonly Body _body;
        private readonly LocatorBodyComponent _locatorBodyComponent;
        private readonly Transform _targetPoint;

        /// <summary>Bodyの参照</summary>
        public Body Body => _body;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraActorView(Body body) {
            _body = body;
            _locatorBodyComponent = body.GetComponent<LocatorBodyComponent>();
            _targetPoint = _locatorBodyComponent.Find("TargetPoint");
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            Object.Destroy(_body.GameObject);
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
            _body.GameObject.SetActive(true);
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
            _body.GameObject.SetActive(false);
        }

        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        void IActorInterface.Update(float deltaTime) {
        }

        /// <summary>
        /// ターゲットのTransform情報を設定
        /// </summary>
        public void SetTargetTransform(Vector3 position, Quaternion rotation) {
            _targetPoint.position = position;
            _targetPoint.rotation = rotation;
        }
    }
}