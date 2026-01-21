using Sample.Core;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用のブラックボード
    /// </summary>
    public class CameraStateBlackboard : IActorStateBlackboard {
        /// <summary>カメラ基準とするターゲット情報</summary>
        public IActorTransform BaseTargetTransform { get; set; }
    }
}