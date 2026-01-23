using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// ヒット検出エンジン
    /// </summary>
    public sealed partial class HitDetectionEngine : IDisposable {
        /// <summary>
        /// ヒット形状種別
        /// </summary>
        internal enum HitShapeType : byte {
            Sphere = 0,
            Box = 1,
        }

        /// <summary>
        /// ヒットのJob計算用のSnapshot
        /// </summary>
        private struct HitSnapshot {
            public int id;
            public int layerMask;
            public HitShapeType shapeType;

            public float3 center;
            public float radius;
            public quaternion rotation;
            public float3 halfExtents;
        }

        /// <summary>
        /// 受けのJob計算用のSnapshot
        /// </summary>
        private struct ReceiveSnapshot {
            public int id;
            public float3 start;
            public float3 end;
            public float radius;
            public int layerMask;
        }

        /// <summary>
        /// 候補ペア
        /// </summary>
        private struct CandidatePair {
            public int hitIndex;
            public int receiveIndex;
        }

        /// <summary>
        /// 判定結果
        /// </summary>
        private struct HitResult {
            public int hitIndex;
            public int receiveIndex;
            public float3 contactPoint;
        }

        /// <summary>
        /// 当たり判定計算用Job
        /// </summary>
        [BurstCompile]
        private struct CheckHitJob : IJobParallelFor {
            [ReadOnly]
            public NativeArray<HitSnapshot> hitSnapshots;
            [ReadOnly]
            public NativeArray<ReceiveSnapshot> receiveSnapshots;
            [ReadOnly]
            public NativeArray<CandidatePair> candidatePairs;

            public int pairCount;

            [WriteOnly]
            public NativeArray<int> hitFlags;
            [WriteOnly]
            public NativeArray<float3> contactPoints;
            [WriteOnly]
            public NativeArray<float3> contactNormals;

            /// <inheritdoc/>
            public void Execute(int index) {
                if (index >= pairCount) {
                    return;
                }

                var pair = candidatePairs[index];
                var hitSnapshot = hitSnapshots[pair.hitIndex];
                var receiveSnapshot = receiveSnapshots[pair.receiveIndex];

                // レイヤー判定
                if ((hitSnapshot.layerMask & receiveSnapshot.layerMask) == 0) {
                    return;
                }

                // 判定
                hitFlags[index] = 0;
                contactPoints[index] = default;
                contactNormals[index] = default;

                switch (hitSnapshot.shapeType) {
                    case HitShapeType.Sphere: {
                        if (TryGetReceiveContactPoint_Sphere(hitSnapshot, receiveSnapshot, out var contactPoint, out var contactNormal)) {
                            hitFlags[index] = 1;
                            contactPoints[index] = contactPoint;
                            contactNormals[index] = contactNormal;
                        }

                        break;
                    }
                    case HitShapeType.Box: {
                        if (TryGetReceiveContactPoint_Box(hitSnapshot, receiveSnapshot, out var contactPoint, out var contactNormal)) {
                            hitFlags[index] = 1;
                            contactPoints[index] = contactPoint;
                            contactNormals[index] = contactNormal;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// ヒットコリジョン登録情報
        /// </summary>
        private sealed class HitEntry<TCollider> {
            public int id;
            public TCollider collider;
            public int layerMask;
            public object customData;
        }

        /// <summary>
        /// 受けコリジョン登録情報
        /// </summary>
        private sealed class ReceiveEntry {
            public int id;
            public IReceiveCollider collider;
            public ICollisionListener listener;
            public int layerMask;
        }

        /// <summary>
        /// 衝突ペアキー
        /// </summary>
        private readonly struct PairKey : IEquatable<PairKey> {
            public readonly int hitId;
            public readonly int receiveId;
            public readonly object customData;

            public PairKey(int hitId, int receiveId, object customData) {
                this.hitId = hitId;
                this.receiveId = receiveId;
                this.customData = customData;
            }

            /// <inheritdoc/>
            public bool Equals(PairKey other) {
                return hitId == other.hitId && receiveId == other.receiveId;
            }

            /// <inheritdoc/>
            public override bool Equals(object obj) {
                return obj is PairKey other && Equals(other);
            }

            /// <inheritdoc/>
            public override int GetHashCode() {
                unchecked {
                    return (hitId * 397) ^ receiveId;
                }
            }
        }

        private readonly UniformGrid2D _hitGrid;
        private readonly List<HitEntry<ISphereHitCollider>> _sphereHitEntries = new();
        private readonly List<HitEntry<IBoxHitCollider>> _boxHitEntries = new();
        private readonly List<ReceiveEntry> _receiveEntries = new();
        private readonly List<object> _customDataList = new();

        private readonly HashSet<PairKey> _prevPariKeys = new();
        private readonly HashSet<PairKey> _currentPariKeys = new();

        private readonly List<int> _tmpCandidates = new(256);

        private NativeArray<HitSnapshot> _hitSnapshots;
        private NativeArray<ReceiveSnapshot> _receiveSnapshots;
        private NativeArray<CandidatePair> _candidatePairs;
        private NativeArray<int> _hitFlags;
        private NativeArray<float3> _contactPoints;
        private NativeArray<float3> _contactNormals;

        private bool _disposed;
        private int _nextHitId = 1;
        private int _nextReceiveId = 1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cellSize">空間分割用のセルサイズ</param>
        public HitDetectionEngine(float cellSize) {
            _hitGrid = new UniformGrid2D(cellSize);
        }

        /// <summary>
        /// 廃棄処理
        /// </summary>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            
            // Debug情報の削除
            DisableDebugView();

            // NativeArrayの解放
            if (_hitSnapshots.IsCreated) {
                _hitSnapshots.Dispose();
            }

            if (_receiveSnapshots.IsCreated) {
                _receiveSnapshots.Dispose();
            }

            if (_candidatePairs.IsCreated) {
                _candidatePairs.Dispose();
            }

            if (_hitFlags.IsCreated) {
                _hitFlags.Dispose();
            }

            if (_contactPoints.IsCreated) {
                _contactPoints.Dispose();
            }

            if (_contactNormals.IsCreated) {
                _contactNormals.Dispose();
            }

            // コリジョン情報をクリア
            _hitGrid.Clear();
            _sphereHitEntries.Clear();
            _receiveEntries.Clear();
        }

        /// <summary>
        /// 球体ヒットコリジョン登録
        /// </summary>
        /// <param name="collider">ヒットコリジョン情報</param>
        /// <param name="layerMask">判定レイヤーマスク</param>
        /// <param name="customData">ヒット検出時に届くカスタムデータ</param>
        public int RegisterHit(ISphereHitCollider collider, int layerMask = ~0, object customData = null) {
            var id = _nextHitId++;
            _sphereHitEntries.Add(new HitEntry<ISphereHitCollider> { id = id, collider = collider, layerMask = layerMask, customData = customData  });
            return id;
        }

        /// <summary>
        /// ボックスヒットコリジョン登録
        /// </summary>
        /// <param name="collider">ヒットコリジョン情報</param>
        /// <param name="layerMask">判定レイヤーマスク</param>
        /// <param name="customData">ヒット検出時に届くカスタムデータ</param>
        public int RegisterHit(IBoxHitCollider collider, int layerMask = ~0, object customData = null) {
            var id = _nextHitId++;
            _boxHitEntries.Add(new HitEntry<IBoxHitCollider> { id = id, collider = collider, layerMask = layerMask, customData = customData });
            return id;
        }

        /// <summary>
        /// 受けコリジョン登録
        /// </summary>
        /// <param name="collider">受けコリジョン情報</param>
        /// <param name="listener">判定検知用リスナー</param>
        /// <param name="layerMask">判定レイヤーマスク</param>
        public int RegisterReceive(IReceiveCollider collider, ICollisionListener listener, int layerMask = ~0) {
            var id = _nextReceiveId++;
            _receiveEntries.Add(new ReceiveEntry { id = id, collider = collider, listener = listener, layerMask = layerMask });
            return id;
        }

        /// <summary>
        /// ヒットコリジョン登録解除
        /// </summary>
        /// <param name="id">登録時のId</param>
        public void UnregisterHit(int id) {
            for (var i = _sphereHitEntries.Count - 1; i >= 0; i--) {
                if (_sphereHitEntries[i].id == id) {
                    _sphereHitEntries.RemoveAt(i);
                    break;
                }
            }

            for (var i = _boxHitEntries.Count - 1; i >= 0; i--) {
                if (_boxHitEntries[i].id == id) {
                    _boxHitEntries.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 受けコリジョン登録解除
        /// </summary>
        /// <param name="id">登録時のId</param>
        public void UnregisterReceive(int id) {
            for (var i = _receiveEntries.Count - 1; i >= 0; i--) {
                if (_receiveEntries[i].id == id) {
                    _receiveEntries.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update() {
            // スナップショット作成
            EnsureNativeArrays();

            var hitWrite = 0;
            _customDataList.Clear();
            for (var i = 0; i < _sphereHitEntries.Count; i++) {
                var entry = _sphereHitEntries[i];
                var collider = entry.collider;
                _customDataList.Add(entry.customData);
                _hitSnapshots[hitWrite++] = new HitSnapshot {
                    id = entry.id,
                    layerMask = entry.layerMask,
                    shapeType = HitShapeType.Sphere,
                    center = collider.Center,
                    radius = collider.Radius,
                    rotation = default,
                    halfExtents = default
                };
            }

            for (var i = 0; i < _boxHitEntries.Count; i++) {
                var entry = _boxHitEntries[i];
                var collider = entry.collider;
                _customDataList.Add(entry.customData);
                _hitSnapshots[hitWrite++] = new HitSnapshot {
                    id = entry.id,
                    layerMask = entry.layerMask,
                    shapeType = HitShapeType.Box,
                    center = collider.Center,
                    radius = 0.0f,
                    rotation = collider.Rotation,
                    halfExtents = collider.HalfExtents
                };
            }

            for (var i = 0; i < _receiveEntries.Count; i++) {
                var entry = _receiveEntries[i];
                var collider = entry.collider;
                _receiveSnapshots[i] = new ReceiveSnapshot {
                    id = entry.id,
                    radius = collider.Radius,
                    start = collider.Start,
                    end = collider.End,
                    layerMask = entry.layerMask,
                };
            }

            // Gridで候補ペア作成
            _hitGrid.Clear();
            for (var hitIndex = 0; hitIndex < hitWrite; hitIndex++) {
                var hitSnapshot = _hitSnapshots[hitIndex];
                switch (hitSnapshot.shapeType) {
                    case HitShapeType.Sphere:
                        _hitGrid.UpsertCircleXZ(hitIndex, hitSnapshot.center, hitSnapshot.radius);
                        break;
                    case HitShapeType.Box:
                        _hitGrid.UpsertObbXZ(hitIndex, hitSnapshot.center, hitSnapshot.rotation, hitSnapshot.halfExtents);
                        break;
                }
            }

            var pairCount = 0;
            for (var receiveIndex = 0; receiveIndex < _receiveEntries.Count; receiveIndex++) {
                var receiveSnapshot = _receiveSnapshots[receiveIndex];
                var queryStart = receiveSnapshot.start;
                var queryEnd = receiveSnapshot.end;
                var queryRadius = receiveSnapshot.radius;

                _hitGrid.QueryCapsuleXZ(queryStart, queryEnd, queryRadius, _tmpCandidates);

                for (var i = 0; i < _tmpCandidates.Count; i++) {
                    var hitIndex = _tmpCandidates[i];

                    if (pairCount >= _candidatePairs.Length) {
                        GrowPairs(pairCount + 1);
                    }

                    _candidatePairs[pairCount++] = new CandidatePair { hitIndex = hitIndex, receiveIndex = receiveIndex };
                }
            }

            // Jobを使った処理の実行
            var job = new CheckHitJob {
                hitSnapshots = _hitSnapshots,
                receiveSnapshots = _receiveSnapshots,
                candidatePairs = _candidatePairs,
                pairCount = pairCount,
                hitFlags = _hitFlags,
                contactPoints = _contactPoints,
                contactNormals = _contactNormals,
            };
            var handle = job.Schedule(pairCount, 64);
            handle.Complete();

            // 通知処理
            _currentPariKeys.Clear();
            for (var i = 0; i < pairCount; i++) {
                if (_hitFlags[i] == 0) {
                    continue;
                }

                var pair = _candidatePairs[i];
                var hitId = _hitSnapshots[pair.hitIndex].id;
                var receiveId = _receiveSnapshots[pair.receiveIndex].id;
                var listener = _receiveEntries[pair.receiveIndex].listener;
                var customData = _customDataList[pair.hitIndex];
                var key = new PairKey(hitId, receiveId, customData);
                _currentPariKeys.Add(key);

                if (listener != null) {
                    var contactPoint = (Vector3)_contactPoints[i];
                    var contactNormal = (Vector3)_contactNormals[i];
                    var evt = new CollisionEvent(hitId, receiveId, contactPoint, contactNormal, customData);
                    if (_prevPariKeys.Contains(key)) {
                        listener.OnCollisionStay(evt);
                    }
                    else {
                        listener.OnCollisionEnter(evt);
                    }
                }
            }

            foreach (var key in _prevPariKeys) {
                if (_currentPariKeys.Contains(key)) {
                    continue;
                }

                var hitId = key.hitId;
                var receiveId = key.receiveId;
                var listener = default(ICollisionListener);
                for (var i = 0; i < _receiveEntries.Count; i++) {
                    if (_receiveEntries[i].id == receiveId) {
                        listener = _receiveEntries[i].listener;
                        break;
                    }
                }

                if (listener != null) {
                    var customData = key.customData;
                    var evt = new CollisionEvent(hitId, receiveId, default, default, customData);
                    listener.OnCollisionExit(evt);
                }
            }

            // ペア集合のスワップ
            _prevPariKeys.Clear();
            foreach (var key in _currentPariKeys) {
                _prevPariKeys.Add(key);
            }
            
            // デバッグ情報のコミット
            CommitDebugFrame(pairCount);
        }

        /// <summary>
        /// 管理対象のNativeArrayの確保
        /// </summary>
        private void EnsureNativeArrays() {
            var totalHitSnapshotCount = _sphereHitEntries.Count + _boxHitEntries.Count;
            Ensure(ref _hitSnapshots, totalHitSnapshotCount);
            Ensure(ref _receiveSnapshots, _receiveEntries.Count);

            // Pairsは適当の初期容量を与える
            if (!_candidatePairs.IsCreated) {
                _candidatePairs = new NativeArray<CandidatePair>(Mathf.Max(256, totalHitSnapshotCount * Mathf.Max(1, _receiveEntries.Count)), Allocator.Persistent);
                _hitFlags = new NativeArray<int>(_candidatePairs.Length, Allocator.Persistent);
                _contactPoints = new NativeArray<float3>(_candidatePairs.Length, Allocator.Persistent);
                _contactNormals = new NativeArray<float3>(_candidatePairs.Length, Allocator.Persistent);
            }
        }

        /// <summary>
        /// ペア情報の再確保
        /// </summary>
        private void GrowPairs(int needed) {
            var newSize = Mathf.NextPowerOfTwo(Mathf.Max(needed, _candidatePairs.Length + 1));

            var newPairs = new NativeArray<CandidatePair>(newSize, Allocator.Persistent);
            var newFlags = new NativeArray<int>(newSize, Allocator.Persistent);
            var newPoints = new NativeArray<float3>(newSize, Allocator.Persistent);
            var newNormals = new NativeArray<float3>(newSize, Allocator.Persistent);

            NativeArray<CandidatePair>.Copy(_candidatePairs, newPairs, _candidatePairs.Length);

            _candidatePairs.Dispose();
            _hitFlags.Dispose();
            _contactPoints.Dispose();
            _contactNormals.Dispose();

            _candidatePairs = newPairs;
            _hitFlags = newFlags;
            _contactPoints = newPoints;
            _contactNormals = newNormals;
        }

        /// <summary>
        /// NativeArrayの確保
        /// </summary>
        private static void Ensure<T>(ref NativeArray<T> array, int length)
            where T : struct {
            if (!array.IsCreated || array.Length != length) {
                if (array.IsCreated) {
                    array.Dispose();
                }

                array = new NativeArray<T>(length, Allocator.Persistent);
            }
        }
    }
}