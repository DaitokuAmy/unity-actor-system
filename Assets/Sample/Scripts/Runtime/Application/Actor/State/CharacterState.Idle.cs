using System.Collections.Generic;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用ステート定義クラス
    /// </summary>
    partial class CharacterStates {
        /// <summary>
        /// 待機状態
        /// </summary>
        public class Idle : CharacterState {
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                Presenter.ChangeIdle();
            }

            /// <inheritdoc/>
            protected override void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                base.Update(commands, signals, deltaTime);

                foreach (var command in commands) {
                    if (command is CharacterCommands.Move move) {
                        Blackboard.MoveVector = move.Value;
                        ChangeState<Locomotion>();
                        return;
                    }

                    if (command is CharacterCommands.Jump jump) {
                        ChangeState<Jump>();
                    }

                    if (command is CharacterCommands.Attack) {
                        Blackboard.AttackIndex = 0;
                        ChangeState<Attack>();
                        return;
                    }
                }
            }
        }
    }
}