using Sample.Application;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// エネミーキャラ操作用クラス
    /// </summary>
    public class EnemyController : CharacterController {
        private float _thinkTimer = 0.0f;
        private Vector2 _moveVector;

        /// <inheritdoc/>
        protected override void Update(float deltaTime) {
            base.Update(deltaTime);
            
            // 思考更新
            UpdateThink(deltaTime);
            
            // 移動反映
            if (_moveVector.sqrMagnitude > 0.0f) {
                var command = Owner.CreateCommand<CharacterCommands.Move>();
                command.Set(_moveVector.x, _moveVector.y);
                Owner.AddCommand(command);;
            }
        }

        /// <summary>
        /// 思考更新
        /// </summary>
        private void UpdateThink(float deltaTime) {
            // 思考タイマー更新
            _thinkTimer -= deltaTime;
            if (_thinkTimer > 0.0f) {
                return;
            }

            // ランダムに移動
            var typeNumber = Random.Range(0, 10);
            switch (typeNumber) {
                case 0:
                    _moveVector = new Vector2(-1.0f, 0.0f);
                    break;
                case 1:
                    _moveVector = new Vector2(1.0f, 0.0f);
                    break;
                case 2:
                    _moveVector = new Vector2(0.0f, -1.0f);
                    break;
                case 3:
                    _moveVector = new Vector2(0.0f, 1.0f);
                    break;
                default:
                    _moveVector = Vector2.zero;
                    break;
            }

            _thinkTimer = 1.0f;
        }
    }
}