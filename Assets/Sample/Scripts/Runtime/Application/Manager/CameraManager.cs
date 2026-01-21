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
        private readonly IActorScheduler<int> _actorScheduler;
        [Inject]
        private readonly ITableAssetStore _tableAssetStore;
        [Inject]
        private readonly ICameraActorFactory _actorFactory;

        private ActorManager<int> _actorManager;

        private int _nextId = 1;

        /// <summary>
        /// Inject処理
        /// </summary>
        [Inject]
        public void Construct(IActorScheduler<int> actorScheduler) {
            _actorManager = new ActorManager<int>(actorScheduler);
        }

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
            model.SetPrefabAssetKey(assetKey);
            actor.SetModel(model);

            await _actorFactory.CreateAsync(actor, model, ct);

            actor.SetupStateMachine(
                typeof(CameraStates.Tps),
                new CameraStates.Tps());

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

        /// <summary>
        /// アクターの取得
        /// </summary>
        public bool TryGetActor(int id, out Actor<int> actor) {
            return _actorManager.TryGetActor(id, out actor);
        }
    }
}