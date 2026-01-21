using Unity.Mathematics;

namespace Sample.Core {
    /// <summary>
    /// ActorのTransformを表すためのインターフェース
    /// </summary>
    public interface IActorTransform {
        /// <summary>所有者Id</summary>
        int OwnerId { get; }
        /// <summary>座標</summary>
        float3 Position { get; }
        /// <summary>向き</summary>
        quaternion Rotation { get; }
    }
}