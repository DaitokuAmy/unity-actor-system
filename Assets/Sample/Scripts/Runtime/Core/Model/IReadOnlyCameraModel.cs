namespace Sample.Core {
    /// <summary>
    /// 読み取り専用のCameraModel
    /// </summary>
    public interface IReadOnlyCameraModel {
        /// <summary>Prefab読み込みなどに使うアセットキー</summary>
        string PrefabAssetKey { get; }
        /// <summary>追従ターゲットのId</summary>
        int TargetId { get; }
        /// <summary>X軸回転</summary>
        float AngleX { get; }
        /// <summary>Y軸回転</summary>
        float AngleY { get; }
    }
}