using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using Sample.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sample.Infrastructure {
    /// <summary>
    /// テーブル情報読み込み管理用アセットストア
    /// </summary>
    public sealed class TableAssetStore : ITableAssetStore {
        private const string RootPath = "Assets/Sample/Gameplay/Table/";

        private PlayerTableData _playerTableData;
        private EnemyTableData _enemyTableData;

        /// <inheritdoc/>
        UniTask ITableAssetStore.LoadTablesAsync(CancellationToken ct) {
            return UniTask.WhenAll(
                LoadTableDataAsync<PlayerTableData>("player").ContinueWith(x => _playerTableData = x),
                LoadTableDataAsync<EnemyTableData>("enemy").ContinueWith(x => _enemyTableData = x)
            );
        }

        /// <inheritdoc/>
        IPlayerMaster ITableAssetStore.FindPlayerById(int id) {
            if (_playerTableData == null) {
                throw new Exception($"{nameof(PlayerTableData)} is not found.");
            }

            return _playerTableData.FindById(id);
        }

        /// <inheritdoc/>
        IEnemyMaster ITableAssetStore.FindEnemyById(int id) {
            if (_enemyTableData == null) {
                throw new Exception($"{nameof(EnemyTableData)} is not found.");
            }

            return _enemyTableData.FindById(id);
        }

        /// <summary>
        /// テーブルデータの読み込み関数
        /// </summary>
        private async UniTask<TTableData> LoadTableDataAsync<TTableData>(string assetKey)
            where TTableData : ScriptableObject {
            var path = $"{RootPath}dat_{assetKey}_table.asset";
            var handle = Addressables.LoadAssetAsync<TTableData>(path);
            await handle.Task;
            return handle.Result;
        }
    }
}