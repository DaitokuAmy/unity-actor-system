using Sample.Core;

namespace Sample.Application {
    /// <summary>
    /// カメラ見た目反映用インターフェース
    /// </summary>
    public interface ICameraPresenter {
        /// <summary>
        /// 注視ターゲットの変更
        /// </summary>
        void ChangeTarget(IAimTarget target);
    }
}