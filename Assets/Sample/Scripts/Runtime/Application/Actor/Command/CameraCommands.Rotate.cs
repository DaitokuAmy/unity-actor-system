using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用コマンド定義クラス
    /// </summary>
    partial class CameraCommands {
        /// <summary>
        /// 向き変更コマンド
        /// </summary>
        public class Rotate : ActorCommand {
            /// <inheritdoc/>
            protected override int Order => (int) CommandOrder.Rotate;
            
            /// <summary>X軸回転量</summary>
            public float DeltaAngleX { get; private set; }
            /// <summary>Y軸回転量</summary>
            public float DeltaAngleY { get; private set; }
            
            /// <summary>
            /// 値の設定
            /// </summary>
            /// <param name="deltaAngleX">X軸回転量</param>
            /// <param name="deltaAngleY">Y軸回転量</param>
            public void Set(float deltaAngleX, float deltaAngleY) {
                DeltaAngleX = deltaAngleX;
                DeltaAngleY = deltaAngleY;
            }
        }
    }
}