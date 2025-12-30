using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Core;

namespace Sample.Application {
    /// <summary>
    /// キャラ生成用のファクトリーインターフェース
    /// </summary>
    public interface ICharacterActorFactory {
        /// <summary>
        /// プレイヤー生成
        /// </summary>
        UniTask CreatePlayerAsync(Actor actor, IReadOnlyPlayerModel model, CancellationToken ct);
    }
}