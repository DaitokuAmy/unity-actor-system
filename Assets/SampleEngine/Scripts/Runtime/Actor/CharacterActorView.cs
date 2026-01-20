using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ制御用ビュー
    /// </summary>
    public class CharacterActorView : IActorView<int> {
        private static readonly int SpeedXPropId = Animator.StringToHash("speed.x");
        private static readonly int SpeedZPropId = Animator.StringToHash("speed.z");
        private static readonly int SpeedPropId = Animator.StringToHash("speed");
        private static readonly int SpeedScalePropId = Animator.StringToHash("speed_scale");
        private static readonly int IsAirPropId = Animator.StringToHash("is_air");

        private readonly Body _body;
        private readonly CharacterActorData _data;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly CharacterController _characterController;
        private readonly RootMotionHandler _rootMotionHandler;
        
        private Vector2 _velocityXZ;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterActorView(Body body, CharacterActorData data) {
            _body = body;
            _data = data;
            _transform = _body.Transform;
            _animator = _body.GetComponent<Animator>();
            _characterController = _body.GetComponent<CharacterController>();
            _rootMotionHandler = _body.GetComponent<RootMotionHandler>();
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            Object.Destroy(_body.GameObject);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _body.GameObject.SetActive(true);
            
            _animator.runtimeAnimatorController = _data.BaseController;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            _body.GameObject.SetActive(false);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) { }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() { }

        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        void IActorInterface<int>.Update(float deltaTime) {
            // var deltaPos = new Vector3(_velocityXZ.x, 0.0f, _velocityXZ.y) * deltaTime;
            // _characterController.Move(deltaPos);
        }

        /// <summary>
        /// 移動速度の設定
        /// </summary>
        /// <param name="x">X軸移動速度</param>
        /// <param name="z">Z軸移動速度</param>
        public void SetVelocity(float x, float z) {
            x *= 10;
            z *= 10;
            _velocityXZ.x = x;
            _velocityXZ.y = z;
            _animator.SetFloat(SpeedXPropId, x);
            _animator.SetFloat(SpeedZPropId, z);
            _animator.SetFloat(SpeedPropId, _velocityXZ.magnitude);
        }

        /// <summary>
        /// 攻撃再生
        /// </summary>
        public async UniTask PlayAttackAsync(CancellationToken ct) {
            _velocityXZ = Vector2.zero;
            // _material.color = Color.red;
            // await using var handle = ct.Register(() => _material.color = Color.white);
            // await UniTask.WaitForSeconds(1.0f, cancellationToken: ct);
            // _material.color = Color.white;
        }
    }
}