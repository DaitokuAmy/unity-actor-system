using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Core;

namespace Sample.Application {
    /// <summary>
    /// カメラ生成用のファクトリーインターフェース
    /// </summary>
    public interface ICameraActorFactory {
        /// <summary>
        /// カメラ生成
        /// </summary>
        UniTask CreateAsync(Actor<int> actor, IReadOnlyCameraModel model, CancellationToken ct);

        /// <summary>
        /// カメラ削除
        /// </summary>
        void Destroy(Actor<int> actor, IReadOnlyCameraModel model);
    }
}