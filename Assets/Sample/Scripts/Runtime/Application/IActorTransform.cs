using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// ActorのTransformを表すためのインターフェース
    /// </summary>
    public interface IActorTransform {
        /// <summary>所有者Id</summary>
        int OwnerId { get; }
        /// <summary>座標</summary>
        Vector3 Position { get; }
        /// <summary>向き</summary>
        Quaternion Rotation { get; }
    }
}