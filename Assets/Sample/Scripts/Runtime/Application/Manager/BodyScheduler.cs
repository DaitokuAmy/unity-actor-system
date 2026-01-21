using System;
using System.Collections.Generic;
using SampleEngine;

namespace Sample.Application {
    /// <summary>
    /// Bodyの更新を行うためのクラス
    /// </summary>
    public sealed class BodyScheduler : IDisposable {
        private readonly List<Body> _bodies = new();
        
        /// <summary>
        /// 廃棄時処理
        /// </summary>
        public void Dispose() {
            _bodies.Clear();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void Update(float deltaTime) {
            foreach (var body in _bodies) {
                body.Update(deltaTime);
            }
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        /// <param name="deltaTime">変位時間</param>
        public void LateUpdate(float deltaTime) {
            foreach (var body in _bodies) {
                body.LateUpdate(deltaTime);
            }
        }

        /// <summary>
        /// Bodyの追加
        /// </summary>
        public void AddBody(Body body) {
            _bodies.Add(body);
        }
        
        /// <summary>
        /// Bodyの除外
        /// </summary>
        public void RemoveBody(Body body) {
            _bodies.Remove(body);
        }
    }
}