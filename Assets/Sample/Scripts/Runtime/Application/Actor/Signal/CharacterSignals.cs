namespace Sample.Application {
    /// <summary>
    /// キャラ用シグナル定義クラス
    /// </summary>
    public static partial class CharacterSignals {
        /// <summary>
        /// 処理順
        /// </summary>
        private enum SignalOrder {
            BeginCombable = 10,
            EndCombable = 11,
        }
    }
}