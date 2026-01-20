using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// キャラアクターデータ
    /// </summary>
    [CreateAssetMenu(fileName = "dat_act_ch000_00.asset", menuName = "Sample Engine/Actor/Character Actor Data")]
    public sealed class CharacterActorData : ScriptableObject {
        [Tooltip("アニメーションの基礎となるController")]
        public RuntimeAnimatorController BaseController;
    }
}