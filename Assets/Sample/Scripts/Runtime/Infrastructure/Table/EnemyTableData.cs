using System;
using Sample.Core;
using SampleEngine;
using UnityEngine;

namespace Sample.Infrastructure {
    /// <summary>
    /// エネミーマスター格納用テーブルデータ
    /// </summary>
    [CreateAssetMenu( fileName = "dat_enemy_table.asset", menuName = "Sample/Table/Enemy")]
    public sealed class EnemyTableData : TableData<int, EnemyTableData.Record> {
        /// <summary>
        /// レコード情報
        /// </summary>
        [Serializable]
        public sealed record Record : IEnemyMaster, IRecord {
            /// <inheritdoc/>
            int IRecord.Id => Id;
            /// <inheritdoc/>
            int ICharacterMaster.Id => Id;
            /// <inheritdoc/>
            string ICharacterMaster.Name => Name;
            /// <inheritdoc/>
            string ICharacterMaster.PrefabAssetKey => PrefabAssetKey;
            /// <inheritdoc/>
            string ICharacterMaster.DataAssetKey => DataAssetKey;

            public int Id;
            public string Name;
            public string PrefabAssetKey;
            public string DataAssetKey;
        }
    }
}