using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Domain;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// カメラマネージャー
    /// </summary>
    public sealed class CameraManager : IDisposable {
        [Inject]
        private readonly ActorManager<int> _actorManager;
        [Inject]
        private readonly ITableAssetStore _tableAssetStore;
        [Inject]
        private readonly ICameraActorFactory _actorFactory;
        
        private int _nextId = 10001;

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        public void Dispose() {
        }

        /// <summary>
        /// カメラ生成
        /// </summary>
        public async UniTask<Actor<int>> CreateAsync(string assetKey, CancellationToken ct) {
            var actorId = _nextId++;
            var actor = _actorManager.CreateActor(actorId);
            var model = new CameraModel();
            actor.SetModel(model);

            await _actorFactory.CreateAsync(actor, model, ct);

            actor.SetupStateMachine(
                typeof(CameraStates.Default),
                new CameraStates.Default());

            actor.SetActive(true);
            return actor;
        }

        /// <summary>
        /// カメラの削除
        /// </summary>
        public void Delete(int id) {
            if (!_actorManager.TryGetActor(id, out var actor)) {
                return;
            }

            var playerModel = actor.GetModel<CameraModel>();
            if (playerModel == null) {
                throw new Exception("CameraModel is not found.");
            }

            _actorFactory.Destroy(actor, playerModel);
            _actorManager.DeleteActor(actor);
        }
    }
}