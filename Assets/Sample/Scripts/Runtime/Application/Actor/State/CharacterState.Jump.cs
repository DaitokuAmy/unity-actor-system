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
        /// ジャンプ状態
        /// </summary>
        public class Jump : CharacterState {
            private UniTask _task;

            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();
                
                // ジャンプアクション再生
                _task = Presenter.PlayJumpActionAsync(CancellationToken);
            }

            /// <inheritdoc/>
            protected override Type Update(IReadOnlyList<ActorCommand> commands, IReadOnlyList<ActorSignal> signals, float deltaTime) {
                var nextType = base.Update(commands, signals, deltaTime);
                if (nextType != null) {
                    return nextType;
                }
                
                // foreach (var signal in signals) {
                // }
                //
                // foreach (var command in commands) {
                // }

                // 演出が終わったら待機に戻る
                if (_task.Status != UniTaskStatus.Pending) {
                    return typeof(Idle);
                }

                return null;
            }
        }
    }
}