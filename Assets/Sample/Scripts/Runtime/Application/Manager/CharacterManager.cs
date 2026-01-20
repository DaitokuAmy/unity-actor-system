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

        private readonly ActorManager<int> _actorManager;
        private int _nextPlayerId = 1;
        private int _nextEnemyId = 1001;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterManager() {
            _actorManager = new ActorManager<int>();
        }

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        public void Dispose() {
            _actorManager.Dispose();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void Update(float deltaTime) {
            _actorManager.UpdateController(deltaTime);
            _actorManager.UpdateStateMachine(deltaTime);
            _actorManager.UpdateModel(deltaTime);
            _actorManager.UpdatePresenter(deltaTime);
            _actorManager.UpdateView(deltaTime);
            _actorManager.UpdateReceiver(deltaTime);
        }

        /// <summary>
        /// プレイヤー生成
        /// </summary>
        public async UniTask<Actor<int>> CreatePlayerAsync(int id, CancellationToken ct) {
            var actorId = _nextPlayerId++;
            var actor = _actorManager.CreateActor(actorId);
            var master = _tableAssetStore.FindPlayerById(id);
            var model = new PlayerModel();
            model.SetMaster(master);
            actor.SetModel(model);
            
            await _actorFactory.CreatePlayerAsync(actor, model, ct);
            
            actor.SetupStateMachine(
                typeof(CharacterStates.Idle),
                new CharacterStates.Idle(),
                new CharacterStates.Locomotion(),
                new CharacterStates.Attack());
            
            actor.SetActive(true);
            return actor;
        }

        /// <summary>
        /// エネミー生成
        /// </summary>
        public async UniTask<Actor<int>> CreateEnemyAsync(int id, CancellationToken ct) {
            var actorId = _nextEnemyId++;
            var actor = _actorManager.CreateActor(actorId);
            var master = _tableAssetStore.FindEnemyById(id);
            var model = new EnemyModel();
            model.SetMaster(master);
            actor.SetModel(model);
            
            await _actorFactory.CreateEnemyAsync(actor, model, ct);
            
            actor.SetupStateMachine(
                typeof(CharacterStates.Idle),
                new CharacterStates.Idle(),
                new CharacterStates.Locomotion(),
                new CharacterStates.Attack());
            
            actor.SetActive(true);
            return actor;
        }
    }
}