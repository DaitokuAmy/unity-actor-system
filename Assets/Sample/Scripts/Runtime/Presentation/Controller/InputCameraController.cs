using System.Threading;
using R3;
using Sample.Application;
using VContainer;

namespace Sample.Presentation {
    /// <summary>
    /// 入力によるカメラ操作クラス
    /// </summary>
    public class InputCameraController : ActorController {
        [Inject]
        private IInputDevice _inputDevice;
        [Inject]
        private CameraService _cameraService;

        /// <inheritdoc/>
        protected override void Activate(CompositeDisposable compositeDisposable, CancellationToken ct) {
            base.Activate(compositeDisposable, ct);
            
            // カメラリセット操作
            _inputDevice.ResetCameraSubject
                .Subscribe(_ => {
                    var command = Owner.CreateCommand<CameraCommands.ResetAngle>();
                    Owner.AddCommand(command);
                })
                .AddTo(compositeDisposable);
        }

        /// <inheritdoc/>
        protected override void Update(float deltaTime) {
            base.Update(deltaTime);

            // 回転操作
            var lookDir = _inputDevice.LookDir;
            if (lookDir.sqrMagnitude > float.Epsilon) {
                var command = Owner.CreateCommand<CameraCommands.Rotate>();
                command.Set(lookDir.y * -0.1f, lookDir.x * 0.1f);
                Owner.AddCommand(command);
            }
        }
    }
}