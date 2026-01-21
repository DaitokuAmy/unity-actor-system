using System;
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
            protected override Type Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                var nextType = base.Update(commands, signals, deltaTime);
                if (nextType != null) {
                    return nextType;
                }
                
                foreach (var command in commands) {
                    if (command is CharacterCommands.Move move) {
                        Presenter.Move(move.Value.x, move.Value.y);
                    }

                    if (command is CharacterCommands.Jump) {
                        return typeof(Jump);
                    }

                    if (command is CharacterCommands.Attack) {
                        Blackboard.AttackIndex = 0;
                        return typeof(Attack);
                    }
                }

                // 特に入力されていなければ待機に戻る
                return typeof(Idle);
            }
        }
    }
}