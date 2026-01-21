using System;
using System.Threading;
using ActionSequencer;
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
        private static readonly int IsFallingPropId = Animator.StringToHash("is_falling");

        private readonly Body _body;
        private readonly CharacterActorData _data;
        private readonly Transform _transform;
        private readonly Animator _animator;
        private readonly CharacterController _characterController;
        private readonly RootMotionHandler _rootMotionHandler;
        private readonly MaterialBodyComponent _materialBodyComponent;
        private readonly MotionBodyComponent _motionBodyComponent;
        private readonly SequenceControllerProvider _sequenceControllerProvider;
        private readonly SequenceController _sequenceController;

        private Vector2 _movementValue;
        private Vector3 _aimPoint;

        /// <summary>Bodyの参照</summary>
        public Body Body => _body;
        /// <summary>読み取り用SequenceController</summary>
        public IReadOnlySequenceController SequenceController => _sequenceController;

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
            _materialBodyComponent = _body.GetComponent<MaterialBodyComponent>();
            _motionBodyComponent = _body.GetComponent<MotionBodyComponent>();
            _sequenceControllerProvider = _body.GetComponent<SequenceControllerProvider>();
            if (_sequenceControllerProvider == null) {
                _sequenceControllerProvider = _body.AddComponent<SequenceControllerProvider>();
            }

            _sequenceController = new SequenceController();
            _sequenceControllerProvider.SetSequenceController(_sequenceController);
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            _sequenceControllerProvider.SetSequenceController(null);
            Object.Destroy(_body.GameObject);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _body.GameObject.SetActive(true);

            _motionBodyComponent.Play(_data.BaseController);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            _sequenceController.StopAll();
            _body.GameObject.SetActive(false);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) { }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() { }

        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        void IActorInterface<int>.Update(float deltaTime) {
            // AnimatorProperty更新
            UpdateAnimatorProperties(deltaTime);

            // シーケンス更新
            _sequenceController.Update(deltaTime);
        }

        /// <summary>
        /// 移動に利用する値の設定
        /// </summary>
        /// <param name="x">X軸移動値</param>
        /// <param name="z">Z軸移動値</param>
        public void SetMoveValue(float x, float z) {
            _movementValue.x = x;
            _movementValue.y = z;
            _movementValue *= _data.SpeedMultiplier;
        }

        /// <summary>
        /// 正面の設定
        /// </summary>
        /// <param name="angleY">正面向きを表すY軸角度</param>
        public void SetForward(float angleY) {
            var eulerAngles = _transform.eulerAngles;
            eulerAngles.y = angleY;
            _transform.eulerAngles = eulerAngles;
        }

        /// <summary>
        /// 攻撃再生
        /// </summary>
        public UniTask PlayAttackAsync(int index, CancellationToken ct) {
            var actions = _data.AttackActions;
            if (index < 0 || index >= actions.Length) {
                return UniTask.CompletedTask;
            }

            SetMoveValue(0, 0);
            return PlayClipActionAsync(actions[index], ct);
        }

        /// <summary>
        /// ジャンプ再生
        /// </summary>
        public UniTask PlayJumpAsync(CancellationToken ct) {
            return PlayClipActionAsync(_data.JumpAction, ct);
        }

        /// <summary>
        /// 汎用AnimationClipアクションの再生
        /// </summary>
        private async UniTask PlayClipActionAsync(CharacterActorData.ClipActionInfo action, CancellationToken ct) {
            _motionBodyComponent.Play(action.Clip, action.InBlend);
            if (action.SequenceClip != null) {
                _sequenceController.Play(action.SequenceClip);
            }

            var duration = action.Clip.length;
            await UniTask.Delay((int)(duration * 1000), cancellationToken: ct);
            _motionBodyComponent.Play(_data.BaseController, action.OutBlend);
        }

        /// <summary>
        /// AnimatorのProperty更新
        /// </summary>
        private void UpdateAnimatorProperties(float deltaTime) {
            void SetFloatBlend(int id, float target, float t) {
                var current = _animator.GetFloat(id);
                _animator.SetFloat(id, Mathf.Lerp(current, target, t));
            }

            // 移動値を相対方向に変換
            var localMovement = new Vector3(_movementValue.x, 0.0f, _movementValue.y);
            localMovement = _transform.InverseTransformVector(localMovement);

            // Motion用プロパティ更新
            SetFloatBlend(SpeedXPropId, localMovement.x, 0.2f);
            SetFloatBlend(SpeedZPropId, localMovement.z, 0.2f);
            SetFloatBlend(SpeedPropId, localMovement.magnitude, 0.2f);
        }
    }
}