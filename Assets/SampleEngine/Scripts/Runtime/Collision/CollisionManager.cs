using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// ヒットコリジョン情報
    /// </summary>
    public interface IHitCollider {
        /// <summary>スフィアの中心</summary>
        Vector3 Center { get; }
        /// <summary>スフィアの半径</summary>
        float Radius { get; }
    }

    /// <summary>
    /// 受けコリジョン情報
    /// </summary>
    public interface IReceiveCollider {
        /// <summary>カプセルの下端座標</summary>
        Vector3 Bottom { get; }
        /// <summary>カプセルの半径</summary>
        float Radius { get; }
        /// <summary>カプセルの高さ</summary>
        float Height { get; }
    }

    /// <summary>
    /// 衝突イベント情報
    /// </summary>
    public readonly struct CollisionEvent {
        /// <summary>Hit側のId</summary>
        public readonly int hitId;
        /// <summary>受け側のId</summary>
        public readonly int receiveId;
        /// <summary>衝突位置</summary>
        public readonly Vector3 contactPoint;

        public CollisionEvent(int hitId, int receiveId, Vector3 contactPoint) {
            this.hitId = hitId;
            this.receiveId = receiveId;
            this.contactPoint = contactPoint;
        }
    }

    /// <summary>
    /// 衝突検知リスナー
    /// </summary>
    public interface ICollisionListener {
        /// <summary>
        /// 衝突開始
        /// </summary>
        void OnCollisionEnter(in CollisionEvent evt);

        /// <summary>
        /// 衝突中
        /// </summary>
        void OnCollisionStay(in CollisionEvent evt);

        /// <summary>
        /// 衝突終了
        /// </summary>
        void OnCollisionExit(in CollisionEvent evt);
    }

    /// <summary>
    /// 当たり判定管理クラス
    /// </summary>
    public sealed class CollisionManager : IDisposable {
        /// <summary>
        /// ヒットのJob計算用のSnapshot
        /// </summary>
        private struct HitSnapshot {
            public int id;
            public float3 center;
            public float radius;
            public int layerMask;
        }

        /// <summary>
        /// 受けのJob計算用のSnapshot
        /// </summary>
        private struct ReceiveSnapshot {
            public int id;
            public float3 bottom;
            public float radius;
            public float height;
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
                if (TryGetReceiveContactPoint(hitSnapshot, receiveSnapshot, out var contactPoint)) {
                    hitFlags[index] = 1;
                    contactPoints[index] = contactPoint;
                }
                else {
                    hitFlags[index] = 0;
                    contactPoints[index] = default;
                }
            }
        }

        /// <summary>
        /// ヒットコリジョン登録情報
        /// </summary>
        private sealed class HitEntry {
            public int id;
            public IHitCollider collider;
            public int layerMask;
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

            public PairKey(int hitId, int receiveId) {
                this.hitId = hitId;
                this.receiveId = receiveId;
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
        private readonly List<HitEntry> _hitEntries = new();
        private readonly List<ReceiveEntry> _receiveEntries = new();

        private readonly HashSet<PairKey> _prevPariKeys = new();
        private readonly HashSet<PairKey> _currentPariKeys = new();

        private readonly List<int> _tmpCandidates = new(256);

        private NativeArray<HitSnapshot> _hitSnapshots;
        private NativeArray<ReceiveSnapshot> _receiveSnapshots;
        private NativeArray<CandidatePair> _candidatePairs;
        private NativeArray<int> _hitFlags;
        private NativeArray<float3> _contactPoints;

        private bool _disposed;
        private int _nextId = 1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="cellSize">空間分割用のセルサイズ</param>
        public CollisionManager(float cellSize) {
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

            // コリジョン情報をクリア
            _hitGrid.Clear();
            _hitEntries.Clear();
            _receiveEntries.Clear();
        }

        /// <summary>
        /// ヒットコリジョン登録
        /// </summary>
        /// <param name="collider">ヒットコリジョン情報</param>
        /// <param name="layerMask">判定レイヤーマスク</param>
        public int RegisterHit(IHitCollider collider, int layerMask = ~0) {
            var id = _nextId++;
            _hitEntries.Add(new HitEntry { id = id, collider = collider, layerMask = layerMask });
            return id;
        }

        /// <summary>
        /// 受けコリジョン登録
        /// </summary>
        /// <param name="collider">受けコリジョン情報</param>
        /// <param name="listener">判定検知用リスナー</param>
        /// <param name="layerMask">判定レイヤーマスク</param>
        public int RegisterReceive(IReceiveCollider collider, ICollisionListener listener, int layerMask = ~0) {
            var id = _nextId++;
            _receiveEntries.Add(new ReceiveEntry { id = id, collider = collider, listener = listener, layerMask = layerMask });
            return id;
        }

        /// <summary>
        /// ヒットコリジョン登録解除
        /// </summary>
        /// <param name="id">登録時のId</param>
        public void UnregisterHit(int id) {
            for (var i = _hitEntries.Count - 1; i >= 0; i--) {
                if (_hitEntries[i].id == id) {
                    _hitEntries.RemoveAt(i);
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

            for (var i = 0; i < _hitEntries.Count; i++) {
                var entry = _hitEntries[i];
                var collider = entry.collider;
                _hitSnapshots[i] = new HitSnapshot {
                    id = entry.id, center = collider.Center, radius = collider.Radius, layerMask = entry.layerMask,
                };
            }

            for (var i = 0; i < _receiveEntries.Count; i++) {
                var entry = _receiveEntries[i];
                var collider = entry.collider;
                _receiveSnapshots[i] = new ReceiveSnapshot {
                    id = entry.id,
                    bottom = collider.Bottom,
                    radius = collider.Radius,
                    height = collider.Height,
                    layerMask = entry.layerMask,
                };
            }

            // Gridで候補ペア作成
            _hitGrid.Clear();
            for (var hitIndex = 0; hitIndex < _hitEntries.Count; hitIndex++) {
                var hitSnapshot = _hitSnapshots[hitIndex];
                _hitGrid.UpsertCircleXZ(hitIndex, hitSnapshot.center, hitSnapshot.radius);
            }

            var pairCount = 0;
            for (var receiveIndex = 0; receiveIndex < _receiveEntries.Count; receiveIndex++) {
                var receiveSnapshot = _receiveSnapshots[receiveIndex];
                var queryCenter = receiveSnapshot.bottom;
                var queryRadius = receiveSnapshot.radius;

                _hitGrid.QueryCircleXZ(queryCenter, queryRadius, _tmpCandidates);

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
                var key = new PairKey(hitId, receiveId);
                _currentPariKeys.Add(key);

                if (listener != null) {
                    var contact = (Vector3)_contactPoints[i];
                    var evt = new CollisionEvent(hitId, receiveId, contact);
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
                    var evt = new CollisionEvent(hitId, receiveId, default);
                    listener.OnCollisionExit(evt);
                }
            }

            // ペア集合のスワップ
            _prevPariKeys.Clear();
            foreach (var key in _currentPariKeys) {
                _prevPariKeys.Add(key);
            }
        }

        /// <summary>
        /// 管理対象のNativeArrayの確保
        /// </summary>
        private void EnsureNativeArrays() {
            Ensure(ref _hitSnapshots, _hitEntries.Count);
            Ensure(ref _receiveSnapshots, _receiveEntries.Count);

            // Pairsは適当の初期容量を与える
            if (!_candidatePairs.IsCreated) {
                _candidatePairs = new NativeArray<CandidatePair>(Mathf.Max(256, _hitEntries.Count * Mathf.Max(1, _receiveEntries.Count)), Allocator.Persistent);
                _hitFlags = new NativeArray<int>(_candidatePairs.Length, Allocator.Persistent);
                _contactPoints = new NativeArray<float3>(_candidatePairs.Length, Allocator.Persistent);
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

            NativeArray<CandidatePair>.Copy(_candidatePairs, newPairs, _candidatePairs.Length);

            _candidatePairs.Dispose();
            _hitFlags.Dispose();
            _contactPoints.Dispose();

            _candidatePairs = newPairs;
            _hitFlags = newFlags;
            _contactPoints = newPoints;
        }

        /// <summary>
        /// NativeArrayの確保
        /// </summary>
        private static void Ensure<T>(ref NativeArray<T> array, int length)
            where T : struct {
            if (length <= 0) {
                if (!array.IsCreated) {
                    array = new NativeArray<T>(0, Allocator.Persistent);
                }

                return;
            }

            if (!array.IsCreated || array.Length != length) {
                if (array.IsCreated) {
                    array.Dispose();
                }

                array = new NativeArray<T>(length, Allocator.Persistent);
            }
        }

        /// <summary>
        /// 当たり判定計算
        /// </summary>
        /// <param name="hit">Sphereコリジョン</param>
        /// <param name="receive">Capsuleコリジョン</param>
        /// <param name="receiveContactPoint">衝突している場合のCapsule表面上の接触点</param>
        /// <returns>衝突している場合 true</returns>
        private static bool TryGetReceiveContactPoint(HitSnapshot hit,　ReceiveSnapshot receive, out float3 receiveContactPoint) {
            var sphereCenter = hit.center;
            var sphereRadius = hit.radius;

            var capsuleBottom = receive.bottom;
            var capsuleRadius = receive.radius;
            var capsuleHeight = receive.height;

            receiveContactPoint = default;

            if (capsuleRadius < 0.0f || sphereRadius < 0.0f || capsuleHeight <= 0.0f) {
                return false;
            }

            // カプセル軸（Y方向）: xz は Bottom の xz 固定、y は [bottom.y, bottom.y + h]
            var axisX = capsuleBottom.x;
            var axisZ = capsuleBottom.z;

            // 「半球中心」の有効範囲に y をクランプする
            // カプセルの球中心は bottom+radius ～ bottom+(h-radius)
            var minY = capsuleBottom.y + capsuleRadius;
            var maxY = capsuleBottom.y + math.max(capsuleRadius, capsuleHeight - capsuleRadius);

            // h < 2r の場合は「球が重なったカプセル」になり得るので、中心区間を潰して対応
            // その場合 minY > maxY になるので、中央値に寄せる
            if (minY > maxY) {
                var mid = capsuleBottom.y + capsuleHeight * 0.5f;
                minY = mid;
                maxY = mid;
            }

            var clampedY = math.clamp(sphereCenter.y, minY, maxY);

            // カプセル軸上最近点 Q（Sphere中心 S に最も近い軸上の点）
            var q = new float3(axisX, clampedY, axisZ);

            // 軸から Sphere中心までのベクトル
            var v = sphereCenter - q;
            var distSq = math.lengthsq(v);

            var sumR = capsuleRadius + sphereRadius;
            var sumRSq = sumR * sumR;

            // 衝突判定（カプセルの「膨張半径」= cr に対して Sphere 半径 sr を足す）
            if (distSq > sumRSq) {
                return false;
            }

            // 衝突点（Receive 側=カプセル表面）
            // 方向が定まらない（中心が軸上）場合は、X+ 方向に倒す（運用に合わせて変更可）
            const float eps = 1e-8f;
            float3 n;
            if (distSq > eps) {
                var invDist = 1.0f / math.rsqrt(distSq);
                n = v * invDist; // Q->S の正規化（カプセル表面の外向き）
            }
            else {
                n = new float3(1.0f, 0.0f, 0.0f);
            }

            receiveContactPoint = q + n * capsuleRadius;
            return true;
        }
    }
}