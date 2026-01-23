using System;
using System.Threading;
using ActionSequencer;
using Cysharp.Threading.Tasks;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ制御用ビュー
    /// </summary>
    public class CharacterActorView : IActorView<int>, IReceiveCollider {
        private static readonly int SpeedXPropId = Animator.StringToHash("speed.x");
        private static readonly int SpeedZPropId = Animator.StringToHash("speed.z");
        private static readonly int DirectionXPropId = Animator.StringToHash("direction.x");
        private static readonly int DirectionZPropId = Animator.StringToHash("direction.z");
        private static readonly int SpeedPropId = Animator.StringToHash("speed");
        private static readonly int SpeedScalePropId = Animator.StringToHash("speed_scale");
        private static readonly int IsFallingPropId = Animator.StringToHash("is_falling");
        private static readonly int LastTagHash = Animator.StringToHash("Last");

        private interface IPlayableSetup<in TPlayable>
            where TPlayable : IPlayable {
            void Apply(TPlayable playable);
        }

        private readonly struct NullSetup<TPlayable> : IPlayableSetup<TPlayable>
            where TPlayable : IPlayable {
            void IPlayableSetup<TPlayable>.Apply(TPlayable playable) { }
        }

        private readonly struct KnockbackSetup : IPlayableSetup<AnimatorControllerPlayable> {
            private readonly float _directionX;
            private readonly float _directionZ;

            public KnockbackSetup(float directionX, float directionZ) {
                _directionX = directionX;
                _directionZ = directionZ;
            }

            void IPlayableSetup<AnimatorControllerPlayable>.Apply(AnimatorControllerPlayable playable) {
                playable.SetFloat(DirectionXPropId, _directionX);
                playable.SetFloat(DirectionZPropId, _directionZ);
            }
        }

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

        /// <inheritdoc/>
        Vector3 IReceiveCollider.Start => Body.Position + Vector3.up * 0.5f;
        /// <inheritdoc/>
        Vector3 IReceiveCollider.End => Body.Position + Vector3.up * 1.5f;
        /// <inheritdoc/>
        float IReceiveCollider.Radius => 0.5f;

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
        /// <param name="index">攻撃Index</param>
        /// <param name="ct">非同期キャンセル用</param>
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
        /// <param name="ct">非同期キャンセル用</param>
        public UniTask PlayJumpAsync(CancellationToken ct) {
            return PlayClipActionAsync(_data.JumpAction, ct);
        }

        /// <summary>
        /// ノックバック再生
        /// </summary>
        /// <param name="damageDirection">ダメージ向き(その方向にノックバックする)</param>
        /// <param name="ct">非同期キャンセル用</param>
        public UniTask PlayKnockbackAsync(Vector3 damageDirection, CancellationToken ct) {
            var localDir = _transform.InverseTransformDirection(damageDirection);
            return PlayControllerActionAsync(_data.KnockbackAction, new KnockbackSetup(localDir.x, localDir.z), ct);
        }

        /// <summary>
        /// 汎用AnimationClipアクションの再生
        /// </summary>
        private async UniTask PlayClipActionAsync(CharacterActorData.ClipActionInfo action, CancellationToken ct) {
            _motionBodyComponent.Play(action.Clip, action.InBlend);
            if (action.SequenceClip != null) {
                _sequenceController.Play(action.SequenceClip);
            }

            var duration = action.Clip.length - action.OutBlend;
            await UniTask.Delay((int)(duration * 1000), cancellationToken: ct);
            _motionBodyComponent.Play(_data.BaseController, action.OutBlend);
        }

        /// <summary>
        /// 汎用AnimatorControllerアクションの再生
        /// </summary>
        private async UniTask PlayControllerActionAsync<TSetup>(CharacterActorData.ControllerActionInfo action, TSetup setup, CancellationToken ct)
            where TSetup : struct, IPlayableSetup<AnimatorControllerPlayable> {
            var playable = _motionBodyComponent.Play(action.Controller, action.InBlend, true);
            setup.Apply(playable);

            if (action.SequenceClip != null) {
                _sequenceController.Play(action.SequenceClip);
            }

            while (true) {
                ct.ThrowIfCancellationRequested();

                var info = playable.GetCurrentAnimatorStateInfo(0);
                if (info.tagHash != LastTagHash) {
                    await UniTask.Yield(PlayerLoopTiming.Update);
                    continue;
                }

                var duration = info.length;
                var currentTime = info.normalizedTime * duration;
                if (currentTime >= (duration - action.OutBlend)) {
                    break;
                }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            _motionBodyComponent.Play(_data.BaseController, action.OutBlend);
        }

        /// <summary>
        /// 汎用AnimatorControllerアクションの再生
        /// </summary>
        private UniTask PlayControllerActionAsync(CharacterActorData.ControllerActionInfo action, CancellationToken ct) {
            return PlayControllerActionAsync(action, default(NullSetup<AnimatorControllerPlayable>), ct);
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