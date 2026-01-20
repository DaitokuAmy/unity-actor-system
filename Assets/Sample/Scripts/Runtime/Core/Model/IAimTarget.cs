using Unity.Mathematics;

namespace Sample.Core {
    /// <summary>
    /// エイムターゲット用インターフェース
    /// </summary>
    public interface IAimTarget {
        /// <summary>座標</summary>
        float3 Position { get; }
        /// <summary>向き</summary>
        quaternion Rotation { get; }
    }
}