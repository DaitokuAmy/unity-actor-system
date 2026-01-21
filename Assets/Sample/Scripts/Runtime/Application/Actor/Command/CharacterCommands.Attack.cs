using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用コマンド定義クラス
    /// </summary>
    partial class CharacterCommands {
        /// <summary>
        /// 攻撃コマンド
        /// </summary>
        public class Attack : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int)CommandOrder.Attack;
        }
    }
}