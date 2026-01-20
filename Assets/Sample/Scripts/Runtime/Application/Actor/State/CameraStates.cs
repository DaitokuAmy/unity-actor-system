using System.Threading;
using Sample.Domain;
using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// カメラ用ステート定義クラス
    /// </summary>
    public static partial class CameraStates {
        /// <summary>
        /// カメラ用ステート基底
        /// </summary>
        public abstract class CameraState : ActorState<int, CameraStateBlackboard> {
            private CancellationTokenSource _cancellationTokenSource;
            
            /// <summary>Model参照</summary>
            protected CameraModel Model { get; private set; }
            /// <summary>Presenter参照</summary>
            protected ICameraPresenter Presenter { get; private set; }
            /// <summary>ステートにいる間有効なキャンセルトークン</summary>
            protected CancellationToken CancellationToken => _cancellationTokenSource?.Token ?? CancellationToken.None;
            
            /// <inheritdoc/>
            protected override void Enter() {
                base.Enter();

                _cancellationTokenSource = new();
                
                Model = Owner.GetModel<CameraModel>();
                Presenter = Owner.GetPresenter<ICameraPresenter>();
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