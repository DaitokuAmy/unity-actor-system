using ActionSequencer;
using Sample.Application;
using SampleEngine;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// ハンドラ
    /// </summary>
    public sealed class CombableRangeSequenceEventHandler : RangeSequenceEventHandler<CombableRangeSequenceEvent> {
        private Actor<int> _owner;

        /// <inheritdoc/>
        protected override void OnEnter(CombableRangeSequenceEvent sequenceEvent) {
            var signal = _owner.CreateSignal<CharacterSignals.BeginCombable>();
            _owner.AddSignal(signal);
        }

        /// <inheritdoc/>
        protected override void OnExit(CombableRangeSequenceEvent sequenceEvent) {
            var signal = _owner.CreateSignal<CharacterSignals.EndCombable>();
            _owner.AddSignal(signal);
        }

        /// <inheritdoc/>
        protected override void OnCancel(CombableRangeSequenceEvent sequenceEvent) {
            OnExit(sequenceEvent);
        }

        /// <summary>
        /// 情報セットアップ
        /// </summary>
        public void Setup(Actor<int> owner) {
            _owner = owner;
        }
    }
}