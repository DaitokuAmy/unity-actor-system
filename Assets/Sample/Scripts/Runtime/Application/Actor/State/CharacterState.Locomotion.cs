using System.Collections.Generic;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用ステート定義クラス
    /// </summary>
    partial class CharacterStates {
        /// <summary>
        /// 移動状態
        /// </summary>
        public class Locomotion : CharacterState {
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                var vector = Blackboard.MoveVector;
                Presenter.Move(vector.x, vector.y);
            }

            /// <inheritdoc/>
            protected override void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                foreach (var command in commands) {
                    if (command is CharacterCommands.Move move) {
                        Presenter.Move(move.Value.x, move.Value.y);
                        return;
                    }

                    if (command is CharacterCommands.Attack) {
                        Blackboard.AttackIndex = 0;
                        ChangeState<Attack>();
                        return;
                    }
                }

                // 移動が押されていなければ待機に戻る
                ChangeState<Idle>();
            }
        }
    }
}