using System;
using System.Collections.Generic;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用ステート定義クラス
    /// </summary>
    partial class CameraStates {
        /// <summary>
        /// Tps状態
        /// </summary>
        public class Tps : CameraState {
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();
                
                if (Blackboard.BaseTargetTransform != null) {
                    Model.SetTargetId(Blackboard.BaseTargetTransform.OwnerId);
                    Presenter.ChangeBaseTarget(Blackboard.BaseTargetTransform);
                }
            }

            /// <inheritdoc/>
            protected override Type Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                var nextType = base.Update(commands, signals, deltaTime);
                if (nextType != null) {
                    return nextType;
                }
                
                foreach (var command in commands) {
                    if (command is CameraCommands.Rotate rotate) {
                        Model.SetAngles(Model.AngleX + rotate.DeltaAngleX, Model.AngleY + rotate.DeltaAngleY);
                    }

                    if (command is CameraCommands.ChangeTarget changeTarget) {
                        Blackboard.BaseTargetTransform = changeTarget.Target;
                        Model.SetTargetId(changeTarget.Target.OwnerId);
                        Presenter.ChangeBaseTarget(changeTarget.Target);
                    }
                }

                return null;
            }
        }
    }
}