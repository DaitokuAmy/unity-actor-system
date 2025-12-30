namespace Sample.Core {
    /// <summary>
    /// 読み取り専用のPlayerModel
    /// </summary>
    public interface IReadOnlyPlayerModel {
        /// <summary>攻撃コンボ最大数</summary>
        int AttackComboMax { get; }
    }
}