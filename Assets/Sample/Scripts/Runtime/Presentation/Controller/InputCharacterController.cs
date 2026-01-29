using System.Threading;
using R3;
using Sample.Application;
using VContainer;

namespace Sample.Presentation {
    /// <summary>
    /// 入力によるキャラ操作クラス
    /// </summary>
    public class InputCharacterController : ActorController {
        [Inject]
        private IInputDevice _inputDevice;
        [Inject]
        private CameraService _cameraService;

        /// <inheritdoc/>
        protected override void Activate(CompositeDisposable compositeDisposable, CancellationToken ct) {
            base.Activate(compositeDisposable, ct);

            // 攻撃入力
            _inputDevice.AttackSubject
                .Subscribe(_ => {
                    var command = CommandInputPort.CreateCommand<CharacterCommands.Attack>();
                    CommandInputPort.AddCommand(command);
                })
                .AddTo(compositeDisposable);

            // ジャンプ入力
            _inputDevice.JumpSubject
                .Subscribe(_ => {
                    var command = CommandInputPort.CreateCommand<CharacterCommands.Jump>();
                    CommandInputPort.AddCommand(command);
                })
                .AddTo(compositeDisposable);
        }

        /// <inheritdoc/>
        protected override void Update(float deltaTime) {
            base.Update(deltaTime);

            // 移動入力
            var moveDir = _inputDevice.MoveDir;
            if (moveDir.sqrMagnitude > float.Epsilon) {
                moveDir = _cameraService.TransformCameraDirection(moveDir.x, moveDir.y);
                var command = CommandInputPort.CreateCommand<CharacterCommands.Move>();
                command.Set(moveDir.x, moveDir.y);
                CommandInputPort.AddCommand(command);
            }
        }
    }
}