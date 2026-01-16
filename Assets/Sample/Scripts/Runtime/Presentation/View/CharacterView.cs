using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ見た目制御用ビュー
    /// </summary>
    public class CharacterView : MonoBehaviour, IActorView<int> {
        private Vector2 _velocityXZ;
        private MeshRenderer _renderer;
        private Material _material;

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            Destroy(gameObject);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            gameObject.SetActive(true);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) { }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) {
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

        /// <summary>
        /// 攻撃再生
        /// </summary>
        public async UniTask PlayAttackAsync(CancellationToken ct) {
            _velocityXZ = Vector2.zero;
            _material.color = Color.red;
            await using var handle = ct.Register(() => _material.color = Color.white);
            await UniTask.WaitForSeconds(1.0f, cancellationToken: ct);
            _material.color = Color.white;
        }

        #region UnityEvent

        private void Awake() {
            _renderer = GetComponent<MeshRenderer>();
            _material = _renderer.material;
        }

        private void Update() { }

        private void LateUpdate() { }

        #endregion
    }
}