using System;
using R3;
using Sample.Application;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sample.Infrastructure {
    /// <summary>
    /// 入力デバイス
    /// </summary>
    public sealed class InputDevice : IInputDevice, IDisposable {
        private readonly InputAction _moveAction;
        private readonly InputAction _lookAction;
        private readonly InputAction _attackAction;
        private readonly InputAction _jumpAction;
        private readonly InputAction _resetCameraAction;
        private readonly Subject<Unit> _attackSubject;
        private readonly Subject<Unit> _jumpSubject;
        private readonly Subject<Unit> _resetCameraSubject;

        private bool _disposed;
        
        /// <inheritdoc/>
        public Vector2 MoveDir { get; private set; }
        /// <inheritdoc/>
        public Vector2 LookDir { get; private set; }

        /// <inheritdoc/>
        public Observable<Unit> AttackSubject => _attackSubject;
        /// <inheritdoc/>
        public Observable<Unit> JumpSubject => _jumpSubject;
        /// <inheritdoc/>
        public Observable<Unit> ResetCameraSubject => _resetCameraSubject;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="playerInput">入力用のPlayerInput</param>
        public InputDevice(PlayerInput playerInput) {
            _moveAction = playerInput.actions["Move"];
            _lookAction = playerInput.actions["Look"];
            _attackAction = playerInput.actions["Attack"];
            _jumpAction = playerInput.actions["Jump"];
            _resetCameraAction = playerInput.actions["ResetCamera"];

            _attackAction.performed += OnAttack;
            _jumpAction.performed += OnJump;
            _resetCameraAction.performed += OnResetCamera;
            
            _attackSubject = new Subject<Unit>();
            _jumpSubject = new Subject<Unit>();
            _resetCameraSubject = new Subject<Unit>();
        }

        /// <inheritdoc/>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            
            _attackSubject.OnCompleted();
            _attackSubject.Dispose();
            _jumpSubject.OnCompleted();
            _jumpSubject.Dispose();
            _resetCameraSubject.OnCompleted();
            _resetCameraSubject.Dispose();
            
            _attackAction.performed -= OnAttack;
            _jumpAction.performed -= OnJump;
            _resetCameraAction.performed -= OnResetCamera;
            
            _moveAction.Dispose();
            _lookAction.Dispose();
            _attackAction.Dispose();
            _jumpAction.Dispose();
            _resetCameraAction.Dispose();
        }

        /// <inheritdoc/>
        public void Update(float deltaTime) {
            if (_disposed) {
                return;
            }
            
            MoveDir = _moveAction.ReadValue<Vector2>();
            LookDir = _lookAction.ReadValue<Vector2>();
        }

        /// <summary>
        /// 攻撃入力通知
        /// </summary>
        private void OnAttack(InputAction.CallbackContext context) {
            _attackSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// ジャンプ入力通知
        /// </summary>
        private void OnJump(InputAction.CallbackContext context) {
            _jumpSubject.OnNext(Unit.Default);
        }

        /// <summary>
        /// カメラリセット入力通知
        /// </summary>
        private void OnResetCamera(InputAction.CallbackContext context) {
            _resetCameraSubject.OnNext(Unit.Default);
        }
    }
}