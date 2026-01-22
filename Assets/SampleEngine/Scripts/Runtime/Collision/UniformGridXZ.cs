using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SampleEngine {
    /// <summary>
    /// 均一グリッド(XZ)
    /// </summary>
    internal sealed class UniformGrid2D {
        private readonly float _cellSize;
        private readonly Dictionary<long, List<int>> _cellToIds = new();
        private readonly Dictionary<int, List<long>> _idToCells = new();
        private readonly ObjectPool<List<long>> _listPool;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UniformGrid2D(float cellSize) {
            _cellSize = cellSize;

            _listPool = new ObjectPool<List<long>>(
                createFunc: () => new List<long>(8),
                actionOnGet: list => list.Clear(),
                actionOnRelease: list => list.Clear(),
                actionOnDestroy: list => list.Clear(),
                defaultCapacity: 64);
        }

        /// <summary>
        /// グリッド内の情報をクリア
        /// </summary>
        public void Clear() {
            foreach (var pair in _idToCells) {
                _listPool.Release(pair.Value);
            }

            _cellToIds.Clear();
            _idToCells.Clear();
        }

        /// <summary>
        /// Idの除外
        /// </summary>
        public void Remove(int id) {
            if (!_idToCells.TryGetValue(id, out var cells)) {
                return;
            }

            for (var i = 0; i < cells.Count; i++) {
                var key = cells[i];
                if (_cellToIds.TryGetValue(key, out var list)) {
                    list.Remove(id);
                    if (list.Count == 0) {
                        _cellToIds.Remove(key);
                    }
                }
            }

            _idToCells.Remove(id);
            _listPool.Release(cells);
        }

        /// <summary>
        /// Circle単位での登録
        /// </summary>
        public void UpsertCircleXZ(int id, Vector3 center, float radius) {
            Remove(id);

            var minX = center.x - radius;
            var maxX = center.x + radius;
            var minZ = center.z - radius;
            var maxZ = center.z + radius;

            var minCx = WorldToCell(minX);
            var maxCx = WorldToCell(maxX);
            var minCz = WorldToCell(minZ);
            var maxCz = WorldToCell(maxZ);

            var keys = _listPool.Get();

            for (var cz = minCz; cz <= maxCz; cz++) {
                for (var cx = minCx; cx <= maxCx; cx++) {
                    var key = Pack(cx, cz);
                    if (!_cellToIds.TryGetValue(key, out var list)) {
                        list = new List<int>(8);
                        _cellToIds[key] = list;
                    }

                    list.Add(id);
                    keys.Add(key);
                }
            }

            _idToCells[id] = keys;
        }

        /// <summary>
        /// Circle範囲の登録Idを列挙
        /// </summary>
        public void QueryCircleXZ(Vector3 center, float radius, List<int> outHitIndices) {
            outHitIndices.Clear();
            
            var minX = center.x - radius;
            var maxX = center.x + radius;
            var minZ = center.z - radius;
            var maxZ = center.z + radius;

            var minCx = WorldToCell(minX);
            var maxCx = WorldToCell(maxX);
            var minCz = WorldToCell(minZ);
            var maxCz = WorldToCell(maxZ);

            var yielded = HashSetPool<int>.Get();
            try {
                for (var cz = minCz; cz <= maxCz; cz++) {
                    for (var cx = minCx; cx <= maxCx; cx++) {
                        var key = Pack(cx, cz);
                        if (!_cellToIds.TryGetValue(key, out var list)) {
                            continue;
                        }

                        for (var i = 0; i < list.Count; i++) {
                            var id = list[i];
                            if (yielded.Add(id)) {
                                outHitIndices.Add(id);
                            }
                        }
                    }
                }
            }
            finally {
                HashSetPool<int>.Release(yielded);
            }
        }

        /// <summary>
        /// ワールド値からセル値に変換
        /// </summary>
        private int WorldToCell(float v) {
            return Mathf.FloorToInt(v / _cellSize);
        }

        /// <summary>
        /// セル情報をHashに変換
        /// </summary>
        private static long Pack(int cx, int cz) {
            unchecked {
                return ((long)cx << 32) | (uint)cz;
            }
        }
    }
}