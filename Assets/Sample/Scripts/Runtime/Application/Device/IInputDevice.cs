using R3;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// 入力デバイス用インターフェース
    /// </summary>
    public interface IInputDevice {
        /// <summary>移動向き</summary>
        Vector2 MoveDir { get; }
        /// <summary>視線移動向き</summary>
        Vector2 LookDir { get; }

        /// <summary>攻撃通知</summary>
        Observable<Unit> AttackSubject { get; }
        /// <summary>ジャンプ通知</summary>
        Observable<Unit> JumpSubject { get; }
        /// <summary>カメラリセット通知</summary>
        Observable<Unit> ResetCameraSubject { get; }

        /// <summary>
        /// 更新処理
        /// </summary>
        void Update(float deltaTime);
    }
}