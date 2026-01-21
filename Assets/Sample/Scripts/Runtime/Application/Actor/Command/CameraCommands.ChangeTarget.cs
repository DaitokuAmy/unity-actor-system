using Sample.Core;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用コマンド定義クラス
    /// </summary>
    partial class CameraCommands {
        /// <summary>
        /// ターゲット変更コマンド
        /// </summary>
        public class ChangeTarget : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int) CommandOrder.ChangeTarget;
            
            /// <summary>ターゲット</summary>
            public IActorTransform Target { get; private set; }
            
            /// <summary>
            /// 値の設定
            /// </summary>
            /// <param name="target">注視ターゲット</param>
            public void Set(IActorTransform target) {
                Target = target;
            }
        }
    }
}