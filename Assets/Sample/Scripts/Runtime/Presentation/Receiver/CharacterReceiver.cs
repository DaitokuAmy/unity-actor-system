using System;
using R3;
using Sample.Application;
using SampleEngine;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// キャライベント監視用クラス
    /// </summary>
    public class CharacterReceiver : IActorReceiver<int> {
        private CompositeDisposable _compositeDisposable;

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

            var sequenceController = ActorView.SequenceController;
            sequenceController.BindRangeEventHandler<TestRangeSequenceEvent, TestRangeSequenceEventHandler>()
                .AddTo(_compositeDisposable);
            sequenceController.BindRangeEventHandler<CombableRangeSequenceEvent>(onEnter: OnEnterCombable, onExit: OnExitCombable, onCancel: OnExitCombable)
                .AddTo(_compositeDisposable);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
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