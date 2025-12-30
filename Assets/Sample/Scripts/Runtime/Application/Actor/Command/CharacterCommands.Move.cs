using UnityActorSystem;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラ用コマンド定義クラス
    /// </summary>
    partial class CharacterCommands {
        /// <summary>
        /// 移動コマンド
        /// </summary>
        public class Move : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int) CommandOrder.Move;
            
            /// <summary>移動量</summary>
            public Vector2 Value { get; private set; }
            
            /// <summary>
            /// 値の設定
            /// </summary>
            /// <param name="x">X軸移動量</param>
            /// <param name="y">Y軸移動量</param>
            public void Set(float x, float y) {
                Value = new Vector2(x, y);
            }
        }
    }
}