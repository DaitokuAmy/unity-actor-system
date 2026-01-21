using System;
using ActionSequencer;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// キャラアクターデータ
    /// </summary>
    [CreateAssetMenu(fileName = "dat_act_ch000_00.asset", menuName = "Sample Engine/Actor/Character Actor Data")]
    public sealed class CharacterActorData : ScriptableObject {
        /// <summary>
        /// 汎用AnimationClipアクション情報
        /// </summary>
        [Serializable]
        public sealed class ClipActionInfo {
            public AnimationClip Clip;
            public SequenceClip SequenceClip;
            public float InBlend = 0.2f;
            public float OutBlend = 0.2f;
        }
        
        /// <summary>
        /// 汎用AnimatorControllerアクション情報
        /// </summary>
        [Serializable]
        public sealed class ControllerActionInfo {
            public RuntimeAnimatorController Controller;
            public SequenceClip SequenceClip;
            public string StartState;
            public float InBlend = 0.2f;
            public float OutBlend = 0.2f;
        }
        
        [Tooltip("アニメーションの基礎となるController")]
        public RuntimeAnimatorController BaseController;
        [Tooltip("移動速度倍率")]
        public float SpeedMultiplier = 10.0f;
        
        [Tooltip("ジャンプアクション")]
        public ClipActionInfo JumpAction;
        [Tooltip("ノックバックアクション")]
        public ControllerActionInfo KnockbackAction;
        [Tooltip("攻撃アクションリスト")]
        public ClipActionInfo[] AttackActions;
    }
}