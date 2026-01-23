using System;
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
            protected override Type Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                var nextType = base.Update(commands, signals, deltaTime);
                if (nextType != null) {
                    return nextType;
                }

                foreach (var signal in signals) {
                    if (signal is CharacterSignals.Hit hit) {
                        Blackboard.KnockbackDirection = hit.AttackParams.direction;
                        return typeof(Knockback);
                    }
                }
                
                foreach (var command in commands) {
                    if (command is CharacterCommands.Move move) {
                        Blackboard.MoveVector = move.Value;
                        return typeof(Locomotion);
                    }

                    if (command is CharacterCommands.Jump) {
                        return typeof(Jump);
                    }

                    if (command is CharacterCommands.Attack) {
                        Blackboard.AttackIndex = 0;
                        return typeof(Attack);
                    }
                }

                return null;
            }
        }
    }
}