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
        /// 汎用アクション情報
        /// </summary>
        [Serializable]
        public sealed class GeneralActionInfo {
            public AnimationClip Clip;
            public SequenceClip SequenceClip;
            public float InBlend = 0.2f;
            public float OutBlend = 0.2f;
        }
        
        [Tooltip("アニメーションの基礎となるController")]
        public RuntimeAnimatorController BaseController;
        [Tooltip("移動速度倍率")]
        public float SpeedMultiplier = 10.0f;
        
        [Tooltip("ジャンプアクション")]
        public GeneralActionInfo JumpAction;
        [Tooltip("攻撃アクションリスト")]
        public GeneralActionInfo[] AttackActions;
    }
}