using Sample.Application;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// 入力によるキャラ操作クラス
    /// </summary>
    public class InputCharacterController : CharacterController {
        /// <inheritdoc/>
        protected override void Update(float deltaTime) {
            base.Update(deltaTime);
            
            // 攻撃判定
            if (Input.GetMouseButtonDown(0)) {
                var command = Owner.CreateCommand<CharacterCommands.Attack>();
                Owner.AddCommand(command);
            }
            
            // ジャンプ判定
            if (Input.GetKeyDown(KeyCode.Space)) {
                var command = Owner.CreateCommand<CharacterCommands.Jump>();
                Owner.AddCommand(command);
            }

            // 移動判定
            var moveVector = Vector2.zero;
            if (Input.GetKey(KeyCode.A)) {
                moveVector.x -= 1.0f;
            }

            if (Input.GetKey(KeyCode.D)) {
                moveVector.x += 1.0f;
            }

            if (Input.GetKey(KeyCode.S)) {
                moveVector.y -= 1.0f;
            }

            if (Input.GetKey(KeyCode.W)) {
                moveVector.y += 1.0f;
            }

            if (moveVector.sqrMagnitude > float.Epsilon) {
                var command = Owner.CreateCommand<CharacterCommands.Move>();
                command.Set(moveVector.x, moveVector.y);
                Owner.AddCommand(command);
            }
        }
    }
}