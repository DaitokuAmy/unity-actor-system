using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sample.Domain;
using UnityActorSystem;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラ用ステート定義クラス
    /// </summary>
    partial class CharacterStates {
        /// <summary>
        /// 攻撃状態
        /// </summary>
        public class Attack : CharacterState {
            private UniTask _task;
            private int _index;
            private bool _canCombo;

            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                _index = Blackboard.AttackIndex;
                _canCombo = false;

                // 攻撃アクション再生
                _task = Presenter.PlayAttackActionAsync(_index, CancellationToken);
                
                Debug.Log($"Attack_{_index}");
            }

            /// <inheritdoc/>
            protected override void Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                foreach (var signal in signals) {
                    if (signal is CharacterSignals.BeginCombable) {
                        _canCombo = true;
                    }

                    if (signal is CharacterSignals.EndCombable) {
                        _canCombo = false;
                    }
                }

                foreach (var command in commands) {
                    if (command is CharacterCommands.Attack && Model is PlayerModel playerModel) {
                        if (_canCombo && _index + 1 < playerModel.AttackComboMax) {
                            Blackboard.AttackIndex = _index + 1;
                            ChangeState<Attack>(true);
                            return;
                        }
                    }
                }

                // 演出が終わったら待機に戻る
                if (_task.Status != UniTaskStatus.Pending) {
                    ChangeState<Idle>();
                }
            }
        }
    }
}