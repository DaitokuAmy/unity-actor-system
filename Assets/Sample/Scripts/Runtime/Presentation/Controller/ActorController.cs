using System;
using System.Threading;
using R3;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// アクター操作用クラス基底
    /// </summary>
    public abstract class ActorController : IActorController {
        private CompositeDisposable _compositeDisposable;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>コマンド発行用ポート</summary>
        protected IActorCommandInputPort CommandInputPort { get; private set; }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
            _compositeDisposable = new CompositeDisposable();
            _cancellationTokenSource = new CancellationTokenSource();
            Activate(_compositeDisposable, _cancellationTokenSource.Token);
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
            Deactivate();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _compositeDisposable.Dispose();
        }

        /// <inheritdoc/>
        void IActorController.Attached(IActorCommandInputPort commandInputPort) {
            CommandInputPort = commandInputPort;
        }

        /// <inheritdoc/>
        void IActorController.Detached() {
            CommandInputPort = null;
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
            Update(deltaTime);
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        protected virtual void Activate(CompositeDisposable compositeDisposable, CancellationToken ct) { }

        /// <summary>
        /// 非アクティブ時処理
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        protected virtual void Update(float deltaTime) { }
    }
}