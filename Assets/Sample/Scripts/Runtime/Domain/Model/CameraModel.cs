using System;
using Sample.Core;
using UnityActorSystem;

namespace Sample.Domain {
    /// <summary>
    /// カメラ用のドメインモデル
    /// </summary>
    public sealed class CameraModel : IActorModel, IReadOnlyCameraModel {
        /// <inheritdoc/>
        public string PrefabAssetKey { get; private set; }
        /// <inheritdoc/>
        public int TargetId { get; private set; }
        /// <inheritdoc/>
        public float AngleX { get; private set; }
        /// <inheritdoc/>
        public float AngleY { get; private set; }

        /// <inheritdoc/>
        void IDisposable.Dispose() {
        }

        /// <inheritdoc/>
        void IActorInterface.Activate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Deactivate() {
        }

        /// <inheritdoc/>
        void IActorInterface.Update(float deltaTime) {
        }

        /// <summary>
        /// プレファブ読み込みなどに使うアセットキーを設定
        /// </summary>
        public void SetPrefabAssetKey(string prefabAssetKey) {
            PrefabAssetKey = prefabAssetKey;
        }

        /// <summary>
        /// 追従ターゲットのIdを設定
        /// </summary>
        public void SetTargetId(int targetId) {
            TargetId = targetId;
        }
        
        /// <summary>
        /// 角度の設定
        /// </summary>
        public void SetAngles(float angleX, float angleY) {
            AngleX = MathF.Min(MathF.Max(angleX, 5.0f), 89.0f);
            AngleY = (angleY + 180.0f) % 360.0f - 180.0f;
        }
        
        /// <summary>
        /// 角度の初期値リセット
        /// </summary>
        public void ResetAngles(float angleY = 0.0f) {
            AngleX = 5.0f;
            AngleY = angleY;
        }
    }
}