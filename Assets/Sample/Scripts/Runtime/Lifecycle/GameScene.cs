using Sample.Application;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Sample.Lifecycle {
    /// <summary>
    /// ゲーム再生用シーン
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class GameScene : LifetimeScope {
        private GameSession _session;
        
        /// <inheritdoc/>
        protected override void Awake() {
            base.Awake();
            
            _session = new GameSession();
            Container.Inject(_session);
        }

        /// <inheritdoc/>
        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);

            builder.Register<CharacterManager>(Lifetime.Singleton);
            builder.Register<ICharacterActorFactory, CharacterActorFactory>(Lifetime.Singleton);
        }

        /// <summary>
        /// アクティブ時処理
        /// </summary>
        private void OnEnable() {
            _session.Start();
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
            _session.Update(Time.deltaTime);
        }
    }
}