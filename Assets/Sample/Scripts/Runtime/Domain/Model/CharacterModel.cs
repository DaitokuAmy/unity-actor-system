using System;
using Sample.Core;
using UnityActorSystem;

namespace Sample.Domain {
    /// <summary>
    /// キャラ用のドメインモデル
    /// </summary>
    public abstract class CharacterModel : IActorModel, IReadOnlyCharacterModel {
        /// <inheritdoc/>
        public int Id { get; private set; }
        /// <inheritdoc/>
        public ICharacterMaster Master { get; private set; }
        
        /// <inheritdoc/>
        void IDisposable.Dispose() {
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
        }

        /// <summary>
        /// セットアップ処理
        /// </summary>
        protected void SetupInternal(int id, ICharacterMaster master) {
            Id = id;
            Master = master;
        }
    }
}