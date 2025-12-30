using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// ゲーム用セッション
    /// </summary>
    public class GameSession {
        [Inject]
        private CharacterManager _characterManager;

        private bool _started;
        private CancellationTokenSource _cancellationTokenSource;
        
        /// <summary>キャンセル用トークン</summary>
        private CancellationToken CancellationToken => _cancellationTokenSource.Token;
        
        /// <summary>
        /// 開始処理
        /// </summary>
        public void Start() {
            if (_started) {
                return;
            }
            
            _started = true;
            _cancellationTokenSource = new();
            
            // キャラ生成
            _characterManager.CreatePlayerAsync(1, CancellationToken).Forget();
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
        /// <param name="deltaTime">変位時間</param>
        public void Update(float deltaTime) {
            if (!_started) {
                return;
            }
            
            _characterManager.Update(deltaTime);
        }
    }
}