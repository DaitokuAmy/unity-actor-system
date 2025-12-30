using System;
using System.Collections.Generic;

namespace Sample {
    /// <summary>
    /// アクター管理用クラス
    /// </summary>
    public sealed class ActorManager<TKey> : IDisposable {
        private readonly List<IActorRuntime> _actorRuntimes = new();
        private readonly Dictionary<TKey, Actor> _actorMap = new();

        private bool _disposed;

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            _actorMap.Clear();
            foreach (var actorRuntime in _actorRuntimes) {
                actorRuntime.Dispose();
            }

            _actorRuntimes.Clear();
        }

        /// <summary>
        /// アクターの生成
        /// </summary>
        public Actor CreateActor(TKey key) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(ActorManager<TKey>));
            }
            
            if (_actorMap.ContainsKey(key)) {
                throw new ArgumentException($"Actor already exists. key={key}");
            }

            var actor = new Actor();
            _actorMap.Add(key, actor);
            _actorRuntimes.Add(actor);
            return actor;
        }

        /// <summary>
        /// アクターの削除
        /// </summary>
        public void DestroyActor(TKey key) {
            if (!_actorMap.Remove(key, out var actor)) {
                return;
            }

            _actorRuntimes.Remove(actor);
            ((IActorRuntime)actor).Dispose();
        }

        /// <summary>
        /// 前半ロジックの更新（Controller/Receiver）
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void UpdatePreLogic(float deltaTime) {
            if (_disposed) {
                return;
            }
            
            foreach (var actorRuntime in _actorRuntimes) {
                actorRuntime.UpdatePreLogic(deltaTime);
            }
        }

        /// <summary>
        /// 後半ロジックの更新（StateMachine/Model）
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void UpdatePostLogic(float deltaTime) {
            if (_disposed) {
                return;
            }
            
            foreach (var actorRuntime in _actorRuntimes) {
                actorRuntime.UpdatePostLogic(deltaTime);
            }
        }

        /// <summary>
        /// ビューの更新（Presenter/View）
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void UpdateView(float deltaTime) {
            if (_disposed) {
                return;
            }
            
            foreach (var actorRuntime in _actorRuntimes) {
                actorRuntime.UpdateView(deltaTime);
            }
        }
    }
}