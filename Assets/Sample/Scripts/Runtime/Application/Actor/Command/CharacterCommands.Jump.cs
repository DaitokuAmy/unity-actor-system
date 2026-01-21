using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用コマンド定義クラス
    /// </summary>
    partial class CharacterCommands {
        /// <summary>
        /// ジャンプコマンド
        /// </summary>
        public class Jump : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int)CommandOrder.Jump;
        }
    }
}