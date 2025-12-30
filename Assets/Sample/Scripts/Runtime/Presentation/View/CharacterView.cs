using System;
using UnityActorSystem;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ見た目制御用ビュー
    /// </summary>
    public class CharacterView : MonoBehaviour, IActorView {
        private Vector2 _velocityXZ;

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            Destroy(gameObject);
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
            gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        void IActorInterface.Attached(Actor actor) { }

        /// <inheritdoc/>
        void IActorInterface.Detached() { }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
            var deltaPos = new Vector3(_velocityXZ.x, 0.0f, _velocityXZ.y) * deltaTime;
            transform.position += deltaPos;
        }

        /// <summary>
        /// 移動速度の設定
        /// </summary>
        /// <param name="x">X軸移動速度</param>
        /// <param name="z">Z軸移動速度</param>
        public void SetVelocity(float x, float z) {
            _velocityXZ.x = x;
            _velocityXZ.y = z;
        }

        #region UnityEvent

        private void Update() { }

        private void LateUpdate() { }

        #endregion
    }
}