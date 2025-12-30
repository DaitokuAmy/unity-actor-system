using UnityActorSystem;

namespace Sample.Application {
    /// <summary>
    /// キャラ用シグナル定義クラス
    /// </summary>
    partial class CharacterSignals {
        /// <summary>
        /// コンボ可能期間開始
        /// </summary>
        public class BeginCombable : ActorSignal {
            /// <inheritdoc/>
            protected override int Order => (int)SignalOrder.BeginCombable;
        }

        /// <summary>
        /// コンボ可能期間終了
        /// </summary>
        public class EndCombable : ActorSignal {
            /// <inheritdoc/>
            protected override int Order => (int)SignalOrder.EndCombable;
        }
    }
}