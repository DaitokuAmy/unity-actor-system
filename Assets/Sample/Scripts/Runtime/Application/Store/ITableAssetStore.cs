using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Core;

namespace Sample.Application {
    /// <summary>
    /// テーブル情報読み込み管理用アセットストア用インターフェース
    /// </summary>
    public interface ITableAssetStore {
        /// <summary>
        /// テーブル情報の読み込み
        /// </summary>
        UniTask LoadTablesAsync(CancellationToken ct);

        /// <summary>
        /// プレイヤーマスターの検索
        /// </summary>
        IPlayerMaster FindPlayerById(int id);

        /// <summary>
        /// エネミーマスターの検索
        /// </summary>
        IEnemyMaster FindEnemyById(int id);
    }
}