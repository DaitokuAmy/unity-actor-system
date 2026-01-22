using System;
using R3;
using Sample.Application;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using VContainer;

namespace Sample.Presentation {
    /// <summary>
    /// キャライベント監視用クラス
    /// </summary>
    public class CharacterReceiver : IActorReceiver<int>, IWorldCollisionListener {
        [Inject]
        private IWorldCollisionService _worldCollisionService;

        private CompositeDisposable _compositeDisposable;
        private int _receiveCollisionId;

        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CharacterActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterReceiver(CharacterActorView actorView) {
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _compositeDisposable = new CompositeDisposable();

            // 受けコリジョン登録
            _receiveCollisionId = _worldCollisionService.RegisterReceive(Owner.Id, ActorView, ~0, this);

            var sequenceController = ActorView.SequenceController;
            sequenceController.BindRangeEventHandler<LogRangeSequenceEvent, LogRangeSequenceEventHandler>()
                .AddTo(_compositeDisposable);
            sequenceController.BindRangeEventHandler<CombableRangeSequenceEvent>(onEnter: OnEnterCombable, onExit: OnExitCombable, onCancel: OnExitCombable)
                .AddTo(_compositeDisposable);
            sequenceController.BindRangeEventHandler<AttackRangeSequenceEvent, AttackRangeSequenceEventHandler>(onInit: handler => {
                    handler.Setup(_worldCollisionService, Owner.Id, ActorView.Body.Transform, ~0);
                })
                .AddTo(_compositeDisposable);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            // 受けコリジョン登録解除
            _worldCollisionService.UnregisterReceive(_receiveCollisionId);

            _compositeDisposable.Dispose();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) { }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionEnter(int hitActorId, int receiveActorId, Vector3 contactPoint) {
            Debug.Log($"OnCollisionEnter: {hitActorId} -> {receiveActorId} ({contactPoint})");
        }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionStay(int hitActorId, int receiveActorId, Vector3 contactPoint) {
            Debug.Log($"OnCollisionStay: {hitActorId} -> {receiveActorId} ({contactPoint})");
        }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionExit(int hitActorId, int receiveActorId) {
            Debug.Log($"OnCollisionExit: {hitActorId} -> {receiveActorId}");
        }

        /// <summary>
        /// コンボ可能エリアの開始
        /// </summary>
        private void OnEnterCombable(CombableRangeSequenceEvent evt) {
            var signal = Owner.CreateSignal<CharacterSignals.BeginCombable>();
            Owner.AddSignal(signal);
        }

        /// <summary>
        /// コンボ可能エリアの終了
        /// </summary>
        private void OnExitCombable(CombableRangeSequenceEvent evt) {
            var signal = Owner.CreateSignal<CharacterSignals.EndCombable>();
            Owner.AddSignal(signal);
        }
    }
}