using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用ステート定義クラス
    /// </summary>
    partial class CharacterStates {
        /// <summary>
        /// ノックバック状態
        /// </summary>
        public class Knockback : CharacterState {
            private UniTask _task;

            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                // 攻撃アクション再生
                _task = Presenter.PlayKnockbackActionAsync(Blackboard.KnockbackDirection, CancellationToken);
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

                // 演出が終わったら待機に戻る
                if (_task.Status != UniTaskStatus.Pending) {
                    return typeof(Idle);
                }

                return null;
            }
        }
    }
}