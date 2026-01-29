using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Domain;
using VContainer;

namespace Sample.Application {
    /// <summary>
    /// キャラマネージャー
    /// </summary>
    public sealed class CharacterManager : IDisposable {
        [Inject]
        private readonly ITableAssetStore _tableAssetStore;
        [Inject]
        private readonly ICharacterActorFactory _actorFactory;

        private ActorManager<int> _actorManager;

        private int _nextPlayerId = 1;
        private int _nextEnemyId = 1001;

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
        /// プレイヤー生成
        /// </summary>
        public async UniTask<Actor<int>> CreatePlayerAsync(int masterId, CancellationToken ct) {
            var actorId = _nextPlayerId++;
            var actor = _actorManager.CreateActor(actorId);
            var master = _tableAssetStore.FindPlayerById(masterId);
            var model = new PlayerModel();
            model.Setup(actorId, master);
            actor.SetModel(model);

            await _actorFactory.CreatePlayerAsync(actor, model, ct);

            actor.SetupStateMachine(
                typeof(CharacterStates.Idle),
                new CharacterStates.Idle(),
                new CharacterStates.Locomotion(),
                new CharacterStates.Attack(),
                new CharacterStates.Jump(),
                new CharacterStates.Knockback());

            actor.SetActive(true);
            return actor;
        }

        /// <summary>
        /// エネミー生成
        /// </summary>
        public async UniTask<Actor<int>> CreateEnemyAsync(int masterId, CancellationToken ct) {
            var actorId = _nextEnemyId++;
            var actor = _actorManager.CreateActor(actorId);
            var master = _tableAssetStore.FindEnemyById(masterId);
            var model = new EnemyModel();
            model.Setup(actorId, master);
            actor.SetModel(model);

            await _actorFactory.CreateEnemyAsync(actor, model, ct);

            actor.SetupStateMachine(
                typeof(CharacterStates.Idle),
                new CharacterStates.Idle(),
                new CharacterStates.Locomotion(),
                new CharacterStates.Attack(),
                new CharacterStates.Jump(),
                new CharacterStates.Knockback());

            actor.SetActive(true);
            return actor;
        }

        /// <summary>
        /// プレイヤーの削除
        /// </summary>
        public void DeletePlayer(int id) {
            if (!_actorManager.TryGetActor(id, out var actor)) {
                return;
            }

            var playerModel = actor.GetModel<PlayerModel>();
            if (playerModel == null) {
                throw new Exception("PlayerModel is not found.");
            }

            _actorFactory.DestroyPlayer(actor, playerModel);
            _actorManager.DeleteActor(actor);
        }

        /// <summary>
        /// エネミーの削除
        /// </summary>
        public void DeleteEnemy(int id) {
            if (!_actorManager.TryGetActor(id, out var actor)) {
                return;
            }

            var playerModel = actor.GetModel<EnemyModel>();
            if (playerModel == null) {
                throw new Exception("PlayerModel is not found.");
            }

            _actorFactory.DestroyEnemy(actor, playerModel);
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