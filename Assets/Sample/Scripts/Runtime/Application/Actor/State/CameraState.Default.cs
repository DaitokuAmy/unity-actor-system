using System.Collections.Generic;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用ステート定義クラス
    /// </summary>
    partial class CameraStates {
        /// <summary>
        /// 通常状態
        /// </summary>
        public class Default : CameraState {
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();
            }

            /// <inheritdoc/>
            protected override void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                base.Update(commands, signals, deltaTime);
            }
        }
    }
}