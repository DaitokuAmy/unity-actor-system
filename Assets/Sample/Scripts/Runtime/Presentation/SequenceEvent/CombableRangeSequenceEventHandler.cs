using ActionSequencer;
using Sample.Application;
using SampleEngine;
using UnityActorSystem;

namespace Sample.Presentation {
    /// <summary>
    /// ハンドラ
    /// </summary>
    public sealed class CombableRangeSequenceEventHandler : RangeSequenceEventHandler<CombableRangeSequenceEvent> {
        private IActorSignalInputPort _signalInputPort;

        /// <inheritdoc/>
        protected override void OnEnter(CombableRangeSequenceEvent sequenceEvent) {
            var signal = _signalInputPort.CreateSignal<CharacterSignals.BeginCombable>();
            _signalInputPort.AddSignal(signal);
        }

        /// <inheritdoc/>
        protected override void OnExit(CombableRangeSequenceEvent sequenceEvent) {
            var signal = _signalInputPort.CreateSignal<CharacterSignals.EndCombable>();
            _signalInputPort.AddSignal(signal);
        }

        /// <inheritdoc/>
        protected override void OnCancel(CombableRangeSequenceEvent sequenceEvent) {
            OnExit(sequenceEvent);
        }

        /// <summary>
        /// 情報セットアップ
        /// </summary>
        public void Setup(IActorSignalInputPort signalInputPort) {
            _signalInputPort = signalInputPort;
        }
    }
}