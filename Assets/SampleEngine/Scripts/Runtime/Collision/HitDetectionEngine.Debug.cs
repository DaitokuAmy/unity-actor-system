using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SampleEngine {
    /// <summary>
    /// ヒット検出エンジン
    /// </summary>
    partial class HitDetectionEngine {
        /// <summary>
        /// デバッグ描画用フレーム（1フレーム分のスナップショット）
        /// </summary>
        internal readonly struct DebugFrame {
            /// <summary>ヒット形状の一覧</summary>
            public readonly IReadOnlyList<DebugHit> hits;
            /// <summary>受け形状の一覧</summary>
            public readonly IReadOnlyList<DebugReceive> receives;
            /// <summary>ヒットしている接触情報の一覧（候補ペアのうち命中したもの）</summary>
            public readonly IReadOnlyList<DebugContact> contacts;
            /// <summary>フレーム番号（更新ごとに増える）</summary>
            public readonly int frameIndex;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DebugFrame(
                IReadOnlyList<DebugHit> hits,
                IReadOnlyList<DebugReceive> receives,
                IReadOnlyList<DebugContact> contacts,
                int frameIndex) {
                this.hits = hits;
                this.receives = receives;
                this.contacts = contacts;
                this.frameIndex = frameIndex;
            }
        }

        /// <summary>
        /// デバッグ用ヒット情報
        /// </summary>
        internal readonly struct DebugHit {
            /// <summary>登録ID</summary>
            public readonly int id;
            /// <summary>判定レイヤーマスク</summary>
            public readonly int layerMask;
            /// <summary>形状種別</summary>
            public readonly HitShapeType shapeType;
            /// <summary>中心</summary>
            public readonly Vector3 center;
            /// <summary>Sphere半径（Sphereのみ）</summary>
            public readonly float radius;
            /// <summary>回転</summary>
            public readonly Quaternion rotation;
            /// <summary>ハーフサイズ</summary>
            public readonly Vector3 halfExtents;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DebugHit(int id, int layerMask, HitShapeType shapeType, Vector3 center, float radius, Quaternion rotation, Vector3 halfExtents) {
                this.id = id;
                this.layerMask = layerMask;
                this.shapeType = shapeType;
                this.center = center;
                this.radius = radius;
                this.rotation = rotation;
                this.halfExtents = halfExtents;
            }
        }

        /// <summary>
        /// デバッグ用受け情報（カプセル）
        /// </summary>
        internal readonly struct DebugReceive {
            /// <summary>登録ID</summary>
            public readonly int id;
            /// <summary>判定レイヤーマスク</summary>
            public readonly int layerMask;
            /// <summary>カプセル線分始点</summary>
            public readonly Vector3 start;
            /// <summary>カプセル線分終点</summary>
            public readonly Vector3 end;
            /// <summary>半径</summary>
            public readonly float radius;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DebugReceive(int id, int layerMask, Vector3 start, Vector3 end, float radius) {
                this.id = id;
                this.layerMask = layerMask;
                this.start = start;
                this.end = end;
                this.radius = radius;
            }
        }

        /// <summary>
        /// デバッグ用接触情報
        /// </summary>
        internal readonly struct DebugContact {
            /// <summary>ヒットID</summary>
            public readonly int hitId;
            /// <summary>受けID</summary>
            public readonly int receiveId;
            /// <summary>接触点（Receive側）</summary>
            public readonly Vector3 point;
            /// <summary>法線（Receive側の外向き）</summary>
            public readonly Vector3 normal;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DebugContact(int hitId, int receiveId, Vector3 point, Vector3 normal) {
                this.hitId = hitId;
                this.receiveId = receiveId;
                this.point = point;
                this.normal = normal;
            }
        }

        private readonly List<DebugHit> _debugHits = new(256);
        private readonly List<DebugReceive> _debugReceives = new(256);
        private readonly List<DebugContact> _debugContacts = new(256);

        private DebugFrame _debugFrame;
        private int _debugFrameIndex;
        private bool _debugEnabled;

        private HitDetectionDebugVisualizer _debugVisualizer;

        /// <summary>
        /// デバッグ描画を有効化
        /// </summary>
        /// <param name="name">生成するGameObject名</param>
        public void EnableDebugView(string name = "HitDetectionEngine(Debug)") {
            if (_disposed) {
                return;
            }
            
            if (_debugEnabled) {
                return;
            }

            _debugEnabled = true;

            var go = new GameObject(name);
            go.hideFlags = HideFlags.DontSave;
            _debugVisualizer = go.AddComponent<HitDetectionDebugVisualizer>();
            _debugVisualizer.Bind(this);
        }

        /// <summary>
        /// デバッグ描画を無効化
        /// </summary>
        public void DisableDebugView() {
            if (!_debugEnabled) {
                return;
            }
            
            _debugEnabled = false;

            if (_debugVisualizer != null) {
                var go = _debugVisualizer.gameObject;
                _debugVisualizer.Unbind();
                _debugVisualizer = null;

                if (go != null) {
                    Object.Destroy(go);
                }
            }
        }

        /// <summary>
        /// デバッグ用フレームを取得します
        /// </summary>
        /// <param name="frame">取得できた場合に返すフレーム</param>
        /// <returns>取得できた場合 true</returns>
        internal bool TryGetDebugFrame(out DebugFrame frame) {
            if (!_debugEnabled) {
                frame = default;
                return false;
            }

            frame = _debugFrame;
            return true;
        }

        /// <summary>
        /// Update() の最後に呼び出して、描画用フレームを確定させます
        /// </summary>
        private void CommitDebugFrame(int pairCount) {
            if (!_debugEnabled) {
                return;
            }

            _debugHits.Clear();
            _debugReceives.Clear();
            _debugContacts.Clear();

            // Hits
            for (var i = 0; i < _hitSnapshots.Length; i++) {
                var hs = _hitSnapshots[i];
                _debugHits.Add(new DebugHit(
                    hs.id,
                    hs.layerMask,
                    hs.shapeType,
                    hs.center,
                    hs.radius,
                    hs.rotation,
                    hs.halfExtents));
            }

            // Receives
            for (var i = 0; i < _receiveSnapshots.Length; i++) {
                var rs = _receiveSnapshots[i];
                _debugReceives.Add(new DebugReceive(
                    rs.id,
                    rs.layerMask,
                    rs.start,
                    rs.end,
                    rs.radius));
            }

            // Contacts（命中分のみ）
            for (var i = 0; i < pairCount; i++) {
                if (_hitFlags[i] == 0) {
                    continue;
                }

                var pair = _candidatePairs[i];
                var hitId = _hitSnapshots[pair.hitIndex].id;
                var receiveId = _receiveSnapshots[pair.receiveIndex].id;

                _debugContacts.Add(new DebugContact(
                    hitId,
                    receiveId,
                    _contactPoints[i],
                    _contactNormals[i]));
            }

            _debugFrameIndex++;
            _debugFrame = new DebugFrame(_debugHits, _debugReceives, _debugContacts, _debugFrameIndex);
        }
    }
}