using Sample.Core;

namespace Sample.Domain {
    /// <summary>
    /// プレイヤー用のドメインモデル
    /// </summary>
    public class PlayerModel : CharacterModel, IReadOnlyPlayerModel {
        /// <inheritdoc/>
        public int AttackComboMax => 1;
    }
}