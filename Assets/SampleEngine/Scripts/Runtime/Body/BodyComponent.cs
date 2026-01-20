using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// Body用のコンポーネント基底
    /// </summary>
    public abstract class BodyComponent : MonoBehaviour, IBodyComponentRuntime {
        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        void IBodyComponentRuntime.Update(float deltaTime) {
            Update(deltaTime);
        }
        
        /// <inheritdoc/>
        // ReSharper disable once Unity.IncorrectMethodSignature
        void IBodyComponentRuntime.LateUpdate(float deltaTime) {
            LateUpdate(deltaTime);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        // ReSharper disable once Unity.IncorrectMethodSignature
        protected virtual void Update(float deltaTime) {
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        // ReSharper disable once Unity.IncorrectMethodSignature
        protected virtual void LateUpdate(float deltaTime) {
        }
    }
}