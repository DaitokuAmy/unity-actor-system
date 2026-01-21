using System;
using System.Collections.Generic;
using UnityActorSystem;
using UnityEngine;

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
                
                // 向きを向き続ける
                UpdateForward(deltaTime);
                
                foreach (var command in commands) {
                    if (command is CharacterCommands.Move move) {
                        Blackboard.MoveVector = move.Value;
                        Presenter.Move(move.Value.x, move.Value.y);
                        return null;
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

            /// <summary>
            /// 正面方向の更新
            /// </summary>
            private void UpdateForward(float deltaTime) {
                var vector = Blackboard.MoveVector;
                if (Mathf.Approximately(vector.x, 0.0f) && Mathf.Approximately(vector.y, 0.0f)) {
                    return;
                }
                
                var current = Presenter.ForwardAngleY;
                var target = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
                if (Mathf.Approximately(current, target)) {
                    return;
                }

                var next = Mathf.MoveTowardsAngle(current, target, 90.0f * deltaTime);
                Presenter.SetForward(next);
            }
        }
    }
}