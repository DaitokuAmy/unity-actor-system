using System;
using System.Threading;
using R3;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// アクター操作用クラス基底
    /// </summary>
    public abstract class ActorController : IActorController<int> {
        private CompositeDisposable _compositeDisposable;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _compositeDisposable = new CompositeDisposable();
            _cancellationTokenSource = new CancellationTokenSource();
            Activate(_compositeDisposable, _cancellationTokenSource.Token);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            Deactivate();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _compositeDisposable.Dispose();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) {
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