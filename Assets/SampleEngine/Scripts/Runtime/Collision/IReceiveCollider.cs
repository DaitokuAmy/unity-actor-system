using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// 受けコリジョン情報
    /// </summary>
    public interface IReceiveCollider {
        /// <summary>カプセルの下端座標</summary>
        Vector3 Bottom { get; }
        /// <summary>カプセルの半径</summary>
        float Radius { get; }
        /// <summary>カプセルの高さ</summary>
        float Height { get; }
    }
}