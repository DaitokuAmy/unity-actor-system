namespace Sample.Core {
    /// <summary>
    /// 読み取り専用のCharacterModel
    /// </summary>
    public interface IReadOnlyCharacterModel {
        /// <summary>マスター情報</summary>
        ICharacterMaster Master { get; }
    }
}