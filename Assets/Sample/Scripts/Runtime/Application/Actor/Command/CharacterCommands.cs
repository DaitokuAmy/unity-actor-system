namespace Sample.Application {
    /// <summary>
    /// キャラ用コマンド定義クラス
    /// </summary>
    public static partial class CharacterCommands {
        /// <summary>
        /// 処理順
        /// </summary>
        private enum CommandOrder {
            Attack = 10,
            Move = 50,
        }
    }
}