using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ見た目反映用クラス
    /// </summary>
    public class CharacterPresenter : ICharacterPresenter, IActorPresenter {
        /// <summary>オーナーアクター</summary>
        protected Actor Owner { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CharacterView View { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterPresenter(CharacterView view) {
            View = view;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface.Activate() { }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() { }

        /// <inheritdoc/>
        void IActorInterface.Attached(Actor actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) { }

        /// <inheritdoc/>
        void ICharacterPresenter.ChangeIdle() {
            View.SetVelocity(0.0f, 0.0f);
        }

        /// <inheritdoc/>
        void ICharacterPresenter.Move(float x, float y) {
            View.SetVelocity(x, y);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayAttackActionAsync(int index, CancellationToken ct) {
            return UniTask.CompletedTask;
        }
    }
}