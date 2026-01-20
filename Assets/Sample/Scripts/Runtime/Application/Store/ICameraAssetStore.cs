using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// カメラアセット読み込み管理用ストア用インターフェース
    /// </summary>
    public interface ICameraAssetStore {
        /// <summary>
        /// カメラプレファブのロード
        /// </summary>
        UniTask<GameObject> LoadCameraPrefabAsync(string key, CancellationToken ct);
    }
}