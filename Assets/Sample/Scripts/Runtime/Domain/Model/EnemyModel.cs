using Sample.Core;

namespace Sample.Domain {
    /// <summary>
    /// エネミー用のドメインモデル
    /// </summary>
    public sealed class EnemyModel : CharacterModel, IReadOnlyEnemyModel {
        /// <summary>
        /// セットアップ処理
        /// </summary>
        public void Setup(int id, ICharacterMaster master) {
            SetupInternal(id, master);
        }
    }
}