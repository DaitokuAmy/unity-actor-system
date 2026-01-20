using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using Sample.Infrastructure;
using UnityActorSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sample.Lifecycle {
    /// <summary>
    /// ゲーム再生用シーン
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class GameScene : LifetimeScope {
        [SerializeField]
        private Transform _characterRoot;

        private GameSession _session;
        private BodyScheduler _bodyScheduler;

        /// <inheritdoc/>
        protected override void Awake() {
            base.Awake();

            _session = new GameSession();
            Container.Inject(_session);
        }

        /// <inheritdoc/>
        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.Register<ITableAssetStore, TableAssetStore>(Lifetime.Singleton);
            builder.Register<ICameraAssetStore, CameraAssetStore>(Lifetime.Singleton);
            builder.Register<ICharacterAssetStore, CharacterAssetStore>(Lifetime.Singleton);
            builder.Register(_ => {
                var manager = new ActorManager<int>();
                return manager;
            },Lifetime.Singleton);
            builder.Register<CharacterManager>(Lifetime.Singleton);
            builder.Register<CameraManager>(Lifetime.Singleton);
            builder.Register<ICharacterActorFactory>(resolver => {
                var factory = new CharacterActorFactory(_characterRoot);
                resolver.Inject(factory);
                return factory;
            }, Lifetime.Singleton);
            builder.Register(resolver => {
                _bodyScheduler = new BodyScheduler();
                resolver.Inject(_bodyScheduler);
                return _bodyScheduler;
            }, Lifetime.Singleton);
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        private void OnEnable() {
            _session.StartAsync(CancellationToken.None).Forget();
        }

        /// <summary>
        /// 非アクティブ時処理
        /// </summary>
        private void OnDisable() {
            _session.Exit();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update() {
            var deltaTime = Time.deltaTime;
            _session?.Update(deltaTime);
            _bodyScheduler?.Update(deltaTime);
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        private void LateUpdate() {
            var deltaTime = Time.deltaTime;
            _bodyScheduler?.LateUpdate(deltaTime);
        }
    }
}