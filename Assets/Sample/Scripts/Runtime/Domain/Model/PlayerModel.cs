using Sample.Core;

namespace Sample.Domain {
    /// <summary>
    /// プレイヤー用のドメインモデル
    /// </summary>
    public class PlayerModel : CharacterModel, IReadOnlyPlayerModel {
        /// <inheritdoc/>
        public int AttackComboMax => 3;

        /// <summary>
        /// セットアップ処理
        /// </summary>
        public void Setup(int id, ICharacterMaster master) {
            SetupInternal(id, master);
        }
    }
}