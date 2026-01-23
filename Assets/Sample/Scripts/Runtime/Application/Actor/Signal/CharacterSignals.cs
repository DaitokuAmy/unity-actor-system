namespace Sample.Application {
    /// <summary>
    /// キャラ用シグナル定義クラス
    /// </summary>
    public static partial class CharacterSignals {
        /// <summary>
        /// 処理順
        /// </summary>
        private enum SignalOrder {
            Hit = 10,
            BeginCombable = 20,
            EndCombable = 21,
        }
    }
}