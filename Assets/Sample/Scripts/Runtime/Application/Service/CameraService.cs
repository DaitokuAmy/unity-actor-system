using Sample.Domain;
using UnityEngine;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// カメラサービス
    /// </summary>
    public sealed class CameraService {
        [Inject]
        private readonly CameraManager _cameraManager;
        [Inject]
        private readonly CharacterManager _characterManager;

        /// <summary>
        /// 追従するターゲットキャラを設定
        /// </summary>
        public void SetTargetCharacter(int cameraId, int targetId) {
            if (!_cameraManager.TryGetActor(cameraId, out var cameraActor)) {
                return;
            }

            if (!_characterManager.TryGetActor(targetId, out var targetActor)) {
                return;
            }

            var targetPresenter = targetActor.GetPresenter<ICharacterPresenter>();
            var command = cameraActor.CreateCommand<CameraCommands.ChangeTarget>();
            command.Set(targetPresenter);
            cameraActor.AddCommand(command);
        }
        
        /// <summary>
        /// X/Zの入力値をカメラ方向を考慮したベクトルに変換する
        /// </summary>
        public Vector2 TransformCameraDirection(int cameraId, float x, float z) {
            if (!_cameraManager.TryGetActor(cameraId, out var cameraActor)) {
                return new Vector2(x, z);
            }

            var model = cameraActor.GetModel<CameraModel>();
            var radian = model.AngleY * Mathf.Deg2Rad;
            
            // 座標変換
            var result = Vector2.zero;
            var cos = Mathf.Cos(radian);
            var sin = Mathf.Sin(radian);
            result.x = x * cos + z * sin;
            result.y = z * cos - x * sin;
            return result;
        }

        /// <summary>
        /// X/Zの入力値をカメラ方向を考慮したベクトルに変換する
        /// </summary>
        public Vector2 TransformCameraDirection(float x, float z) {
            return TransformCameraDirection(1, x, z);
        }
    }
}