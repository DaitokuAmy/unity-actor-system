using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using Sample.Core;
using UnityActorSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ見た目反映用クラス
    /// </summary>
    public class CharacterPresenter : ICharacterPresenter {
        /// <summary>所有者Id</summary>
        int IActorTransform.OwnerId => Model.Id;
        /// <inheritdoc/>
        Vector3 IActorTransform.Position => ActorView.Body.Position;
        /// <inheritdoc/>
        Quaternion IActorTransform.Rotation => ActorView.Body.Rotation;
        /// <inheritdoc/>
        float ICharacterPresenter.ForwardAngleY => ActorView.Body.Transform.eulerAngles.y;

        /// <summary>モデル</summary>
        protected IReadOnlyCharacterModel Model { get; }
        /// <summary>制御用のビュー</summary>
        protected CharacterActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterPresenter(IReadOnlyCharacterModel model, CharacterActorView actorView) {
            Model = model;
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface.Activate() { }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() { }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) { }

        /// <inheritdoc/>
        void ICharacterPresenter.ChangeIdle() {
            ActorView.SetMoveValue(0.0f, 0.0f);
        }

        /// <inheritdoc/>
        void ICharacterPresenter.Move(float x, float y) {
            ActorView.SetMoveValue(x, y);
        }

        /// <inheritdoc/>
        void ICharacterPresenter.SetForward(float angleY) {
            ActorView.SetForward(angleY);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayAttackActionAsync(int index, CancellationToken ct) {
            return ActorView.PlayAttackAsync(index, ct);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayJumpActionAsync(CancellationToken ct) {
            return ActorView.PlayJumpAsync(ct);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayKnockbackActionAsync(Vector3 damageDirection, CancellationToken ct) {
            return ActorView.PlayKnockbackAsync(damageDirection, ct);
        }
    }
}