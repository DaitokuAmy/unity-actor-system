using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用コマンド定義クラス
    /// </summary>
    partial class CameraCommands {
        /// <summary>
        /// 向きリセットコマンド
        /// </summary>
        public class ResetAngle : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int) CommandOrder.ResetAngle;
        }
    }
}