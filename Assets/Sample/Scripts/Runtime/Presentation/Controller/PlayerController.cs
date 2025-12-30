using Sample.Application;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// プレイヤーキャラ操作用クラス
    /// </summary>
    public class PlayerController : CharacterController {
        /// <inheritdoc/>
        protected override void Update(float deltaTime) {
            base.Update(deltaTime);
            
            // 攻撃判定
            if (Input.GetKeyDown(KeyCode.Space)) {
                var command = Owner.CreateCommand<CharacterCommands.Attack>();
                Owner.AddCommand(command);
            }

            // 移動判定
            var moveVector = Vector2.zero;
            if (Input.GetKey(KeyCode.LeftArrow)) {
                moveVector.x -= 1.0f;
            }

            if (Input.GetKey(KeyCode.RightArrow)) {
                moveVector.x += 1.0f;
            }

            if (Input.GetKey(KeyCode.DownArrow)) {
                moveVector.y -= 1.0f;
            }

            if (Input.GetKey(KeyCode.UpArrow)) {
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