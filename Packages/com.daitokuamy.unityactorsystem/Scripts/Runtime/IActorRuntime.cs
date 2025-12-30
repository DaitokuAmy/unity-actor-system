using System;

namespace UnityActorSystem {
    /// <summary>
    /// アクター更新用のインターフェース
    /// </summary>
    internal interface IActorRuntime : IDisposable {
        /// <summary>
        /// 前半ロジック更新
        /// </summary>
        void UpdatePreLogic(float deltaTime);

        /// <summary>
        /// 後半ロジック更新
        /// </summary>
        void UpdatePostLogic(float deltaTime);

        /// <summary>
        /// ビュー更新
        /// </summary>
        void UpdateView(float deltaTime);
    }
}