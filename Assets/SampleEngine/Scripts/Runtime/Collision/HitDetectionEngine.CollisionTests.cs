using Unity.Mathematics;

namespace SampleEngine {
    /// <summary>
    /// ヒット検出エンジン
    /// </summary>
    partial class HitDetectionEngine {
        /// <summary>
        /// 当たり判定計算(Sphere hit vs Capsule receive)
        /// </summary>
        /// <param name="hit">ヒットコリジョン（Sphere）</param>
        /// <param name="receive">受けコリジョン（Capsule: start/end/radius）</param>
        /// <param name="contactPoint">衝突している場合のCapsule表面上の接触点</param>
        /// <param name="contactNormal">衝突している場合のCapsule表面の外向き法線（Hit方向）</param>
        /// <returns>衝突している場合 true</returns>
        private static bool TryGetReceiveContactPoint_Sphere(
            HitSnapshot hit,
            ReceiveSnapshot receive,
            out float3 contactPoint,
            out float3 contactNormal) {

            var sphereCenter = hit.Center;
            var sphereRadius = hit.Radius;

            var capsuleA = receive.Start;
            var capsuleB = receive.End;
            var capsuleRadius = receive.Radius;

            contactPoint = default;
            contactNormal = default;

            if (capsuleRadius < 0.0f || sphereRadius < 0.0f) {
                return false;
            }

            // カプセル軸線分上の最近点 Q
            var q = ClosestPointOnSegment(capsuleA, capsuleB, sphereCenter);

            // 軸から Sphere中心までのベクトル
            var v = sphereCenter - q;
            var distSq = math.lengthsq(v);

            var sumR = capsuleRadius + sphereRadius;
            var sumRSq = sumR * sumR;

            if (distSq > sumRSq) {
                return false;
            }

            // Receive 側の外向き法線（軸→Sphere方向）
            // 方向が定まらない（球中心が軸上）場合は、線分方向に直交する安定方向を作る
            contactNormal = SafeNormalizeFromSegment(v, capsuleA, capsuleB);

            // Receive 側（カプセル表面）接触点
            contactPoint = q + contactNormal * capsuleRadius;
            return true;
        }

        /// <summary>
        /// 当たり判定計算(OBB hit vs Capsule receive)
        /// </summary>
        /// <param name="hit">ヒットコリジョン（OBB）</param>
        /// <param name="receive">受けコリジョン（Capsule: start/end/radius）</param>
        /// <param name="contactPoint">衝突している場合のCapsule表面上の接触点</param>
        /// <param name="contactNormal">衝突している場合のCapsule表面の外向き法線（Hit方向）</param>
        /// <returns>衝突している場合 true</returns>
        private static bool TryGetReceiveContactPoint_Box(
            HitSnapshot hit,
            ReceiveSnapshot receive,
            out float3 contactPoint,
            out float3 contactNormal) {

            contactPoint = default;
            contactNormal = default;

            var capsuleAWorld = receive.Start;
            var capsuleBWorld = receive.End;
            var capsuleRadius = receive.Radius;

            if (capsuleRadius < 0f) {
                return false;
            }

            // OBBローカルへ（OBB中心=原点、回転=Identity）
            var invRot = math.inverse(hit.Rotation);

            var a = math.mul(invRot, capsuleAWorld - hit.Center);
            var b = math.mul(invRot, capsuleBWorld - hit.Center);

            // OBBはローカルでは AABB: [-e, +e]
            var e = hit.HalfExtents;

            // 線分上の点を複数評価して AABB への最近点を取る（軽量・実用版）
            // ※厳密な Segment-AABB 最短距離へ差し替える余地あり
            var bestDistSq = float.PositiveInfinity;
            var bestAxisPointLocal = default(float3); // 線分上の点（ローカル）
            var bestBoxPointLocal = default(float3);  // AABB上の最近点（ローカル）

            Evaluate(0f);
            Evaluate(1f);
            Evaluate(0.5f);

            // 面交点っぽい候補
            AddPlaneCandidates(a, b, e.x, 0);
            AddPlaneCandidates(a, b, e.y, 1);
            AddPlaneCandidates(a, b, e.z, 2);

            if (bestDistSq > capsuleRadius * capsuleRadius) {
                return false;
            }

            // ワールドへ戻す（軸点・箱点）
            var axisPointWorld = hit.Center + math.mul(hit.Rotation, bestAxisPointLocal);
            var boxPointWorld = hit.Center + math.mul(hit.Rotation, bestBoxPointLocal);

            // Receive 側外向き法線：カプセル軸→ヒット（箱）方向
            var v = boxPointWorld - axisPointWorld;
            contactNormal = SafeNormalizeFromSegment(v, capsuleAWorld, capsuleBWorld);

            // Receive 側表面点
            contactPoint = axisPointWorld + contactNormal * capsuleRadius;
            return true;

            void Evaluate(float t) {
                t = math.clamp(t, 0f, 1f);
                var p = math.lerp(a, b, t);         // 線分上（ローカル）
                var c = ClampToAabb(p, e);          // AABB最近点（ローカル）
                var d = p - c;
                var dsq = math.lengthsq(d);

                if (dsq < bestDistSq) {
                    bestDistSq = dsq;
                    bestAxisPointLocal = p;
                    bestBoxPointLocal = c;
                }
            }

            void AddPlaneCandidates(float3 p0, float3 p1, float extent, int axis) {
                var d = p1 - p0;
                var denom = axis == 0 ? d.x : axis == 1 ? d.y : d.z;
                if (math.abs(denom) < 1e-8f) {
                    return;
                }

                var p0A = axis == 0 ? p0.x : axis == 1 ? p0.y : p0.z;

                var tPos = ( extent - p0A) / denom;
                var tNeg = (-extent - p0A) / denom;

                if (tPos >= 0f && tPos <= 1f) Evaluate(tPos);
                if (tNeg >= 0f && tNeg <= 1f) Evaluate(tNeg);
            }

            static float3 ClampToAabb(float3 p, float3 he) {
                return new float3(
                    math.clamp(p.x, -he.x, he.x),
                    math.clamp(p.y, -he.y, he.y),
                    math.clamp(p.z, -he.z, he.z)
                );
            }
        }

        /// <summary>
        /// 線分 AB 上の点で、点 P に最も近い点を返します。
        /// </summary>
        private static float3 ClosestPointOnSegment(float3 a, float3 b, float3 p) {
            var ab = b - a;
            var abLenSq = math.lengthsq(ab);

            if (abLenSq <= 1e-12f) {
                return a;
            }

            var t = math.dot(p - a, ab) / abLenSq;
            t = math.clamp(t, 0f, 1f);
            return a + ab * t;
        }

        /// <summary>
        /// v を正規化して返します。長さがほぼゼロの場合は、線分方向から安定した代替ベクトルを作って返します。
        /// </summary>
        private static float3 SafeNormalizeFromSegment(float3 v, float3 segA, float3 segB) {
            var lenSq = math.lengthsq(v);
            const float eps = 1e-8f;

            if (lenSq > eps) {
                return v * math.rsqrt(lenSq);
            }

            // v がゼロに近い：線分方向に直交する方向を作る
            var segDir = segB - segA;
            var segLenSq = math.lengthsq(segDir);

            if (segLenSq <= eps) {
                // 線分も潰れているなら固定方向
                return new float3(1f, 0f, 0f);
            }

            segDir *= math.rsqrt(segLenSq);

            // segDir と平行になりにくい基底を選ぶ
            var basis = math.abs(segDir.y) < 0.99f ? new float3(0f, 1f, 0f) : new float3(1f, 0f, 0f);

            // segDir と basis の外積で直交ベクトル
            var n = math.cross(segDir, basis);
            var nLenSq = math.lengthsq(n);

            if (nLenSq <= eps) {
                return new float3(1f, 0f, 0f);
            }

            return n * math.rsqrt(nLenSq);
        }
    }
}