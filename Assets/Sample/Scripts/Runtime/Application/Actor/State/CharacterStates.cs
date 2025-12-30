using System.Threading;
using Sample.Domain;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用ステート定義クラス
    /// </summary>
    public static partial class CharacterStates {
        /// <summary>
        /// キャラ用ステート基底
        /// </summary>
        public abstract class CharacterState : ActorState<CharacterStateBlackboard> {
            private CancellationTokenSource _cancellationTokenSource;
            
            /// <summary>Model参照</summary>
            protected PlayerModel Model { get; private set; }
            /// <summary>Presenter参照</summary>
            protected ICharacterPresenter Presenter { get; private set; }
            /// <summary>ステートにいる間有効なキャンセルトークン</summary>
            protected CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
            
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                _cancellationTokenSource = new();
                
                Model = Owner.GetModel<PlayerModel>();
                Presenter = Owner.GetPresenter<ICharacterPresenter>();
            }

            /// <inheritdoc/>
            protected override void Exit() {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                
                base.Exit();
            }
        }
    }
}