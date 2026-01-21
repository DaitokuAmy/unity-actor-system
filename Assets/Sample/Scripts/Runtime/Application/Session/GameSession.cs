using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using UnityEngine;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// ゲーム用セッション
    /// </summary>
    public class GameSession {
        [Inject]
        private readonly BodyScheduler _bodyScheduler;
        [Inject]
        private readonly ActorScheduler<int> _actorScheduler;
        [Inject]
        private readonly ITableAssetStore _tableAssetStore;
        [Inject]
        private readonly CameraManager _cameraManager;
        [Inject]
        private readonly CharacterManager _characterManager;
        [Inject]
        private readonly CameraService _cameraService;
        [Inject]
        private readonly IInputDevice _inputDevice;

        private bool _started;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>キャンセル用トークン</summary>
        private CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// 開始処理
        /// </summary>
        public async UniTask StartAsync(CancellationToken ct) {
            if (_started) {
                return;
            }

            _started = true;
            _cancellationTokenSource = new();

            var linkedCt = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, ct).Token;

            // マスター読み込み
            await _tableAssetStore.LoadTablesAsync(linkedCt);
            
            // カメラ生成
            var camActor = await _cameraManager.CreateAsync("cam001", linkedCt);

            // プレイヤー生成
            var playerActor = await _characterManager.CreatePlayerAsync(1, linkedCt);
            
            // カメラターゲット初期化
            _cameraService.SetTargetCharacter(camActor.Id, playerActor.Id);

            // エネミー生成
            for (var i = 0; i < 4; i++) {
                await _characterManager.CreateEnemyAsync(1, linkedCt);
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Exit() {
            if (!_started) {
                return;
            }

            _started = false;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update() {
            if (!_started) {
                return;
            }

            var deltaTime = Time.deltaTime;
            _inputDevice.Update(deltaTime);
            _actorScheduler.PreUpdate(deltaTime);
            _actorScheduler.UpdateLogic(deltaTime);
            _actorScheduler.UpdatePresentation(deltaTime);
            _bodyScheduler.Update(deltaTime);
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        public void LateUpdate() {
            if (!_started) {
                return;
            }

            var deltaTime = Time.deltaTime;
            _actorScheduler.PostUpdate(deltaTime);
            _bodyScheduler.LateUpdate(deltaTime);
        }
    }
}