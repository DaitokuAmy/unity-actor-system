namespace Sample.Core {
    /// <summary>
    /// キャラマスターアクセス用インターフェース
    /// </summary>
    public interface ICharacterMaster {
        /// <summary>識別子</summary>
        int Id { get; }
        /// <summary>名称</summary>
        string Name { get; }
        /// <summary>プレファブ用アセットキー</summary>
        string PrefabAssetKey { get; }
        /// <summary>制御データ用アセットキー</summary>
        string DataAssetKey { get; }
    }
}