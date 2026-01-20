using ActionSequencer;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// テスト用の範囲シーケンスイベント
    /// </summary>
    public class TestRangeSequenceEvent : RangeSequenceEvent {
        public string Label = "Hoge";
    }

    /// <summary>
    /// ハンドラ
    /// </summary>
    public class TestRangeSequenceEventHandler : RangeSequenceEventHandler<TestRangeSequenceEvent> {
        /// <inheritdoc/>
        protected override void OnEnter(TestRangeSequenceEvent sequenceEvent) {
            Debug.Log($"Enter:{sequenceEvent.Label}");
        }

        /// <inheritdoc/>
        protected override void OnExit(TestRangeSequenceEvent sequenceEvent) {
            Debug.Log($"Exit:{sequenceEvent.Label}");
        }
    }
}