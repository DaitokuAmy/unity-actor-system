using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using Sample.Core;
using Unity.Mathematics;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャラ見た目反映用クラス
    /// </summary>
    public class CharacterPresenter : ICharacterPresenter, IActorPresenter<int> {
        /// <summary>所有者Id</summary>
        int IActorTransform.OwnerId => Owner.Id;
        /// <inheritdoc/>
        float3 IActorTransform.Position => ActorView.Body.Position;
        /// <inheritdoc/>
        quaternion IActorTransform.Rotation => ActorView.Body.Rotation;

        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CharacterActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterPresenter(CharacterActorView actorView) {
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) { }

        /// <inheritdoc/>
        void ICharacterPresenter.ChangeIdle() {
            ActorView.SetMoveValue(0.0f, 0.0f);
        }

        /// <inheritdoc/>
        void ICharacterPresenter.Move(float x, float y) {
            ActorView.SetMoveValue(x, y);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayAttackActionAsync(int index, CancellationToken ct) {
            return ActorView.PlayAttackAsync(index, ct);
        }

        /// <inheritdoc/>
        UniTask ICharacterPresenter.PlayJumpActionAsync(CancellationToken ct) {
            return ActorView.PlayJumpAsync(ct);
        }
    }
}