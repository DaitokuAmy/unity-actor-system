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
        private readonly ICharacterActorFactory _actorFactory;

        private readonly ActorManager<int> _actorManager;

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
            _actorManager.UpdatePreLogic(deltaTime);
            _actorManager.UpdatePostLogic(deltaTime);
            _actorManager.UpdateView(deltaTime);
        }

        /// <summary>
        /// プレイヤー生成
        /// </summary>
        public async UniTask<Actor> CreatePlayerAsync(int id, CancellationToken ct) {
            var actor = _actorManager.CreateActor(id);
            
            var model = new PlayerModel();
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
    }
}