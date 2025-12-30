using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Sample {
    /// <summary>
    /// アクター用のバッファ基底
    /// </summary>
    public abstract class ActorBuffer<TContent> : IDisposable
        where TContent : class, IActorBufferContent, new() {
        private readonly Dictionary<Type, ObjectPool<TContent>> _poolMap = new();
        private readonly List<TContent> _bufferContents = new();
        private readonly List<TContent> _gotContents = new();

        private bool _disposed;

        /// <summary>
        /// 廃棄時処理
        /// </summary>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;

            ClearBuffer();
            
            foreach (var content in _gotContents) {
                _poolMap[content.GetType()].Release(content);
            }

            _gotContents.Clear();

            foreach (var pool in _poolMap.Values) {
                pool.Dispose();
            }
            
            _poolMap.Clear();
        }

        /// <summary>
        /// PoolからBufferContentを取得
        /// </summary>
        protected T GetContentFromPool<T>()
            where T : TContent, new() {
            var type = typeof(T);
            if (!_poolMap.TryGetValue(type, out var pool)) {
                pool = new ObjectPool<TContent>(() => {
                    var instance = new T();
                    instance.OnCreated();
                    return instance;
                }, actionOnRelease: content => content.OnReleased());
                _poolMap.Add(type, pool);
            }

            var instance = (T)pool.Get();
            _gotContents.Add(instance);
            return instance;
        }

        /// <summary>
        /// BufferContentの追加
        /// </summary>
        /// <param name="content">追加対象のContent</param>
        protected void AddContent(TContent content) {
            _bufferContents.Add(content);
            _gotContents.Remove(content);
        }

        /// <summary>
        /// Bufferのクリア
        /// </summary>
        protected void ClearBuffer() {
            foreach (var content in _bufferContents) {
                var pool = _poolMap[content.GetType()];
                pool.Release(content);
            }

            _bufferContents.Clear();
        }
    }
}