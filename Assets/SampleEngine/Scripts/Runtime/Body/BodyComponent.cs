using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// Body用のコンポーネント基底
    /// </summary>
    public abstract class BodyComponent : MonoBehaviour, IBodyComponentRuntime {
        /// <inheritdoc/>
        void IBodyComponentRuntime.Tick(float deltaTime) {
            Tick(deltaTime);
        }
        
        /// <inheritdoc/>
        void IBodyComponentRuntime.PostTick(float deltaTime) {
            PostTick(deltaTime);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        protected virtual void Tick(float deltaTime) {
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        protected virtual void PostTick(float deltaTime) {
        }
    }
}