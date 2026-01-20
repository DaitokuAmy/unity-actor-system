using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace SampleEngine {
    /// <summary>
    /// モーション制御用コンポーネント
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class MotionBodyComponent : BodyComponent {
        private const int MixerPortCount = 2;

        private Animator _animator;

        private PlayableGraph _playableGraph;
        private AnimationMixerPlayable _mixer;
        private int _currentPortIndex = -1;
        private float _blendTimer = 0.0f;
        private Dictionary<RuntimeAnimatorController, AnimatorControllerPlayable> _animatorControllerPlayableCache = new();
        private Dictionary<AnimationClip, AnimationClipPlayable> _animationClipPlayableCache = new();

        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        protected override void Tick(float deltaTime) {
            // Blend処理
            if (_blendTimer >= 0.0f) {
                _blendTimer -= deltaTime;
                var t = _blendTimer > deltaTime ? deltaTime / _blendTimer : 1.0f;
                for (var i = 0; i < MixerPortCount; i++) {
                    var target = i == _currentPortIndex ? 1.0f : 0.0f;
                    var weight = Mathf.Lerp(_mixer.GetInputWeight(i), target, t);
                    _mixer.SetInputWeight(i, weight);
                }
            }
        }

        /// <summary>
        /// 生成時処理
        /// </summary>
        private void Awake() {
            _animator = GetComponent<Animator>();
            _playableGraph = PlayableGraph.Create();
            var output = AnimationPlayableOutput.Create(_playableGraph, "Output", _animator);
            _mixer = AnimationMixerPlayable.Create(_playableGraph, MixerPortCount);
            output.SetSourcePlayable(_mixer);
            _playableGraph.Play();
        }

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        private void OnDestroy() {
            ClearCaches();
            _playableGraph.Destroy();
        }

        /// <summary>
        /// キャッシュクリア
        /// </summary>
        public void ClearCaches() {
            foreach (var val in _animatorControllerPlayableCache.Values) {
                val.Destroy();
            }

            _animatorControllerPlayableCache.Clear();
            foreach (var val in _animationClipPlayableCache.Values) {
                val.Destroy();
            }

            _animationClipPlayableCache.Clear();
        }

        /// <summary>
        /// AnimatorControllerベースでの再生
        /// </summary>
        public AnimatorControllerPlayable Play(RuntimeAnimatorController controller, float blendDuration = 0.0f) {
            if (!_animatorControllerPlayableCache.TryGetValue(controller, out var playable)) {
                playable = AnimatorControllerPlayable.Create(_playableGraph, controller);
                _animatorControllerPlayableCache[controller] = playable;
            }

            playable.SetTime(0.0f);
            Play(playable, blendDuration);
            return playable;
        }

        /// <summary>
        /// AnimationClipベースでの再生
        /// </summary>
        public AnimationClipPlayable Play(AnimationClip clip, float blendDuration = 0.0f) {
            if (!_animationClipPlayableCache.TryGetValue(clip, out var playable)) {
                playable = AnimationClipPlayable.Create(_playableGraph, clip);
                _animationClipPlayableCache[clip] = playable;
            }

            playable.SetTime(0.0f);
            Play(playable, blendDuration);
            return playable;
        }

        /// <summary>
        /// 生成済みPlayableでの再生
        /// </summary>
        public void Play(Playable playable, float blendDuration = 0.0f) {
            _currentPortIndex = (_currentPortIndex + 1) % MixerPortCount;
            _mixer.DisconnectInput(_currentPortIndex);
            _mixer.ConnectInput(_currentPortIndex, playable, 0, 0.0f);
            _blendTimer = blendDuration;
        }
    }
}