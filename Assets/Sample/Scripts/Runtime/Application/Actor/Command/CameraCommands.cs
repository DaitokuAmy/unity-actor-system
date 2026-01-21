namespace Sample.Application {
    /// <summary>
    /// カメラ用コマンド定義クラス
    /// </summary>
    public static partial class CameraCommands {
        /// <summary>
        /// 処理順
        /// </summary>
        private enum CommandOrder {
            ChangeTarget = 10,
            Rotate = 10,
        }
    }
}