namespace SampleEngine {
    /// <summary>
    /// Body用のコンポーネントインターフェース
    /// </summary>
    public interface IBodyComponentRuntime {
        /// <summary>
        /// 更新
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// 後更新
        /// </summary>
        void LateUpdate(float deltaTime);
    }
}