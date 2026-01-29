namespace Sample.Core {
    /// <summary>
    /// 読み取り専用のCharacterModel
    /// </summary>
    public interface IReadOnlyCharacterModel {
        /// <summary>識別Id</summary>
        int Id { get; }
        /// <summary>マスター情報</summary>
        ICharacterMaster Master { get; }
    }
}