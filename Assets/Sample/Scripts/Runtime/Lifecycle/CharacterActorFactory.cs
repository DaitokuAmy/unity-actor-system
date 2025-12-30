using System.Threading;
using Cysharp.Threading.Tasks;
using UnityActorSystem;
using Sample.Application;
using Sample.Core;
using Sample.Presentation;
using UnityEngine;

namespace Sample.Lifecycle {
    /// <summary>
    /// Character関連のActorFactory実装
    /// </summary>
    public sealed class CharacterActorFactory : ICharacterActorFactory {
        /// <summary>
        /// プレイヤーの生成
        /// </summary>
        public UniTask CreatePlayerAsync(Actor actor, IReadOnlyPlayerModel model, CancellationToken ct) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var view = cube.AddComponent<CharacterView>();
            actor.SetView(view);
            var controller = new PlayerController();
            actor.SetController(controller);
            var presenter = new CharacterPresenter(view);
            actor.SetPresenter(presenter);
            return UniTask.CompletedTask;
        }
    }
}