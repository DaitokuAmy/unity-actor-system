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
        UniTask CreatePlayerAsync(Actor<int> actor, IReadOnlyPlayerModel model, CancellationToken ct);
        
        /// <summary>
        /// エネミー生成
        /// </summary>
        UniTask CreateEnemyAsync(Actor<int> actor, IReadOnlyEnemyModel model, CancellationToken ct);

        /// <summary>
        /// プレイヤー削除
        /// </summary>
        void DestroyPlayer(Actor<int> actor, IReadOnlyPlayerModel model);
        
        /// <summary>
        /// エネミー削除
        /// </summary>
        void DestroyEnemy(Actor<int> actor, IReadOnlyEnemyModel model);
    }
}