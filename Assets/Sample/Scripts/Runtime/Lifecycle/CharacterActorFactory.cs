using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Application;
using Sample.Core;
using Sample.Presentation;
using SampleEngine;
using UnityEngine;
using VContainer;

namespace Sample.Lifecycle {
    /// <summary>
    /// Character関連のActorFactory実装
    /// </summary>
    public sealed class CharacterActorFactory : ICharacterActorFactory {
        [Inject]
        private IObjectResolver _objectResolver;
        [Inject]
        private ICharacterAssetStore _characterAssetStore;
        [Inject]
        private BodyScheduler _bodyScheduler;

        private readonly Transform _rootTransform;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterActorFactory(Transform rootTransform) {
            _rootTransform = rootTransform;
        }

        /// <inheritdoc/>
        async UniTask ICharacterActorFactory.CreatePlayerAsync(Actor<int> actor, IReadOnlyPlayerModel model, CancellationToken ct) {
            // 読み込み
            var prefab = await _characterAssetStore.LoadCharacterPrefabAsync(model.Master.PrefabAssetKey, ct);
            var actorData = await _characterAssetStore.LoadCharacterActorDataAsync(model.Master.DataAssetKey, ct);

            // 生成/初期化
            var gameObj = Object.Instantiate(prefab, _rootTransform, false);
            gameObj.name = gameObj.name.Replace("(Clone)", $"_Player{model.Master.Id:D4}");
            var body = new Body(gameObj);
            _bodyScheduler.AddBody(body);
            var view = new CharacterActorView(body, actorData);
            actor.SetView(view);
            var controller = new InputCharacterController();
            _objectResolver.Inject(controller);
            actor.SetController(controller);
            var presenter = new CharacterPresenter(view);
            _objectResolver.Inject(presenter);
            actor.SetPresenter(presenter);
            var receiver = new CharacterReceiver(model, view);
            _objectResolver.Inject(receiver);
            actor.SetReceiver(receiver);
        }

        /// <inheritdoc/>
        async UniTask ICharacterActorFactory.CreateEnemyAsync(Actor<int> actor, IReadOnlyEnemyModel model, CancellationToken ct) {
            // 読み込み
            var prefab = await _characterAssetStore.LoadCharacterPrefabAsync(model.Master.PrefabAssetKey, ct);
            var actorData = await _characterAssetStore.LoadCharacterActorDataAsync(model.Master.DataAssetKey, ct);

            // 生成/初期化
            var gameObj = Object.Instantiate(prefab, _rootTransform, false);
            gameObj.name = gameObj.name.Replace("(Clone)", $"_Enemy{model.Master.Id:D4}");
            var body = new Body(gameObj);
            _bodyScheduler.AddBody(body);
            var view = new CharacterActorView(body, actorData);
            actor.SetView(view);
            var controller = new AICharacterController();
            _objectResolver.Inject(controller);
            actor.SetController(controller);
            var presenter = new CharacterPresenter(view);
            _objectResolver.Inject(presenter);
            actor.SetPresenter(presenter);
            var receiver = new CharacterReceiver(model, view);
            _objectResolver.Inject(receiver);
            actor.SetReceiver(receiver);
        }

        /// <inheritdoc/>
        void ICharacterActorFactory.DestroyPlayer(Actor<int> actor, IReadOnlyPlayerModel model) {
            var view = actor.GetView<CharacterActorView>();
            _bodyScheduler.RemoveBody(view.Body);
        }

        /// <inheritdoc/>
        void ICharacterActorFactory.DestroyEnemy(Actor<int> actor, IReadOnlyEnemyModel model) {
            var view = actor.GetView<CharacterActorView>();
            _bodyScheduler.RemoveBody(view.Body);
        }
    }
}