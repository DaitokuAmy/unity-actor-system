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
        private ICharacterAssetStore _characterAssetStore;

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
            gameObj.name = $"Player_{model.Master.Id:D4}";
            var body = new Body(gameObj);
            var view = new CharacterActorView(body, actorData);
            actor.SetView(view);
            var controller = new InputCharacterController();
            actor.SetController(controller);
            var presenter = new CharacterPresenter(view);
            actor.SetPresenter(presenter);
            var receiver = new CharacterReceiver(view);
            actor.SetReceiver(receiver);
        }
        
        /// <inheritdoc/>
        async UniTask ICharacterActorFactory.CreateEnemyAsync(Actor<int> actor, IReadOnlyEnemyModel model, CancellationToken ct) {
            // 読み込み
            var prefab = await _characterAssetStore.LoadCharacterPrefabAsync(model.Master.PrefabAssetKey, ct);
            var actorData = await _characterAssetStore.LoadCharacterActorDataAsync(model.Master.DataAssetKey, ct);
            
            // 生成/初期化
            var gameObj = Object.Instantiate(prefab, _rootTransform, false);
            gameObj.name = $"Enemy_{model.Master.Id:D4}";
            var body = new Body(gameObj);
            var view = new CharacterActorView(body, actorData);
            actor.SetView(view);
            var controller = new AICharacterController();
            actor.SetController(controller);
            var presenter = new CharacterPresenter(view);
            actor.SetPresenter(presenter);
            var receiver = new CharacterReceiver(view);
            actor.SetReceiver(receiver);
        }
    }
}