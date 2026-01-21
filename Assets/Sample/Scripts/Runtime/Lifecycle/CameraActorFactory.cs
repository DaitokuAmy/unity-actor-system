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
    /// Camera関連のActorFactory実装
    /// </summary>
    public sealed class CameraActorFactory : ICameraActorFactory {
        [Inject]
        private IObjectResolver _objectResolver;
        [Inject]
        private ICameraAssetStore _cameraAssetStore;
        [Inject]
        private BodyScheduler _bodyScheduler;

        private readonly Transform _rootTransform;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraActorFactory(Transform rootTransform) {
            _rootTransform = rootTransform;
        }

        /// <inheritdoc/>
        async UniTask ICameraActorFactory.CreateAsync(Actor<int> actor, IReadOnlyCameraModel model, CancellationToken ct) {
            // 読み込み
            var prefab = await _cameraAssetStore.LoadCameraPrefabAsync(model.PrefabAssetKey, ct);

            // 生成/初期化
            var gameObj = Object.Instantiate(prefab, _rootTransform, false);
            gameObj.name = gameObj.name.Replace("(Clone)", "");
            var body = new Body(gameObj);
            _bodyScheduler.AddBody(body);
            var view = new CameraActorView(body);
            actor.SetView(view);
            var controller = new InputCameraController();
            _objectResolver.Inject(controller);
            actor.SetController(controller);
            var presenter = new CameraPresenter(model, view);
            _objectResolver.Inject(presenter);
            actor.SetPresenter(presenter);
        }

        /// <inheritdoc/>
        void ICameraActorFactory.Destroy(Actor<int> actor, IReadOnlyCameraModel model) {
            var view = actor.GetView<CameraActorView>();
            _bodyScheduler.RemoveBody(view.Body);
        }
    }
}