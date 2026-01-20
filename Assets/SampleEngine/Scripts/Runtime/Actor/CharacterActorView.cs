using System;
using System.Threading;
using ActionSequencer;
using Cysharp.Threading.Tasks;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using UnityEngine.Playables;
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
        private readonly SequenceController _sequenceController;

        private Vector2 _velocityXZ;

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

            _sequenceController = new SequenceController();
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
            Object.Destroy(_body.GameObject);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _body.GameObject.SetActive(true);

            _motionBodyComponent.Play(_data.BaseController);
            _materialBodyComponent.SetColor("Full", _data.BaseColor);
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
            void SetFloatBlend(int id, float target, float t) {
                var current = _animator.GetFloat(id);
                _animator.SetFloat(id, Mathf.Lerp(current, target, t));
            }

            // Motion用プロパティ更新
            SetFloatBlend(SpeedXPropId, _velocityXZ.x, 0.2f);
            SetFloatBlend(SpeedZPropId, _velocityXZ.y, 0.2f);
            SetFloatBlend(SpeedPropId, _velocityXZ.magnitude, 0.2f);

            // シーケンス更新
            _sequenceController.Update(deltaTime);
        }

        /// <summary>
        /// 移動速度の設定
        /// </summary>
        /// <param name="x">X軸移動速度</param>
        /// <param name="z">Z軸移動速度</param>
        public void SetVelocity(float x, float z) {
            _velocityXZ.x = x;
            _velocityXZ.y = z;
            _velocityXZ *= _data.SpeedMultiplier;
        }

        /// <summary>
        /// 攻撃再生
        /// </summary>
        public UniTask PlayAttackAsync(int index, CancellationToken ct) {
            var actions = _data.AttackActions;
            if (index < 0 || index >= actions.Length) {
                return UniTask.CompletedTask;
            }

            SetVelocity(0, 0);
            return PlayGeneralActionAsync(actions[index], ct);
        }

        /// <summary>
        /// ジャンプ再生
        /// </summary>
        public UniTask PlayJumpAsync(CancellationToken ct) {
            return PlayGeneralActionAsync(_data.JumpAction, ct);
        }

        /// <summary>
        /// 汎用アクションの再生
        /// </summary>
        private async UniTask PlayGeneralActionAsync(CharacterActorData.GeneralActionInfo action, CancellationToken ct) {
            _motionBodyComponent.Play(action.Clip, action.InBlend);
            if (action.SequenceClip != null) {
                _sequenceController.Play(action.SequenceClip);
            }

            var duration = action.Clip.length;
            await UniTask.Delay((int)(duration * 1000), cancellationToken: ct);
            _motionBodyComponent.Play(_data.BaseController, action.OutBlend);
        }
    }
}