using System.Collections.Generic;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// テーブルデータ基底
    /// </summary>
    public abstract class TableData<TKey, TRecord> : ScriptableObject
        where TRecord : class, TableData<TKey, TRecord>.IRecord {
        /// <summary>
        /// レコード用インターフェース
        /// </summary>
        public interface IRecord {
            TKey Id { get; }
        }

        [SerializeField]
        private TRecord[] _records;

        private Dictionary<TKey, TRecord> _recordMap;
        
        /// <summary>読み取り用のレコード情報</summary>
        public IReadOnlyList<TRecord> Records => _records;

        /// <summary>
        /// レコードの検索
        /// </summary>
        public TRecord FindById(TKey id) {
            if (_recordMap == null) {
                _recordMap = new Dictionary<TKey, TRecord>();
                foreach (var record in _records) {
                    _recordMap[record.Id] = record;
                }
            }

            if (_recordMap.TryGetValue(id, out var foundRecord)) {
                return foundRecord;
            }

            return null;
        }

        /// <summary>
        /// データ更新検知
        /// </summary>
        private void OnValidate() {
            _recordMap = null;
        }
    }
}