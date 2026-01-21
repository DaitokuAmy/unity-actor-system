using Sample.Core;

namespace Sample.Application {
    /// <summary>
    /// カメラ見た目反映用インターフェース
    /// </summary>
    public interface ICameraPresenter {
        /// <summary>
        /// 基準ターゲットの変更
        /// </summary>
        void ChangeBaseTarget(IActorTransform targetTransform);
    }
}