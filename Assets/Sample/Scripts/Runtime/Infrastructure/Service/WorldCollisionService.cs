using System.Collections.Generic;
using Sample.Application;
using SampleEngine;
using UnityEngine.Pool;

namespace Sample.Infrastructure {
    /// <summary>
    /// 世界のコリジョン管理を提供するサービス
    /// </summary>
    public sealed class WorldCollisionService : IWorldCollisionService, ICollisionListener {
        private readonly HitDetectionEngine _hitDetectionEngine;
        private readonly Dictionary<int, int> _hidIdToActorIds = new();
        private readonly Dictionary<int, int> _receiveIdToActorIds = new();
        private readonly Dictionary<int, List<int>> _actorIdToHitIds = new();
        private readonly Dictionary<int, List<int>> _actorIdToReceiveIds = new();
        private readonly Dictionary<int, IWorldCollisionListener> _receiveIdToListeners = new();
        private readonly ObjectPool<List<int>> _listPool;

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionEnter(in CollisionEvent evt) {
            if (!_hidIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_receiveIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_receiveIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionEnter(hitActorId, receiveActorId, evt.contactPoint);
        }

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionStay(in CollisionEvent evt) {
            if (!_hidIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_receiveIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_receiveIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionStay(hitActorId, receiveActorId, evt.contactPoint);
        }

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionExit(in CollisionEvent evt) {
            if (!_hidIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_receiveIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_receiveIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionExit(hitActorId, receiveActorId);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorldCollisionService() {
            _listPool = new ObjectPool<List<int>>(() => new List<int>(), list => list.Clear(), list => list.Clear());
            _hitDetectionEngine = new HitDetectionEngine(5.0f);
        }

        /// <inheritdoc/>
        void IWorldCollisionService.Update(float deltaTime) {
            _hitDetectionEngine.Update();
        }

        /// <inheritdoc/>
        int IWorldCollisionService.RegisterReceive(int actorId, IReceiveCollider collider, int layerMask, IWorldCollisionListener listener) {
            var receiveId = _hitDetectionEngine.RegisterReceive(collider, this, layerMask);
            _receiveIdToActorIds[receiveId] = actorId;
            _receiveIdToListeners[receiveId] = listener;
            if (!_actorIdToReceiveIds.TryGetValue(actorId, out var list)) {
                list = _listPool.Get();
                _actorIdToReceiveIds[actorId] = list;
            }

            list.Add(receiveId);
            return receiveId;
        }

        /// <inheritdoc/>
        int IWorldCollisionService.RegisterHit(int actorId, ISphereHitCollider collider, int layerMask) {
            var hitId = _hitDetectionEngine.RegisterHit(collider, layerMask);
            _hidIdToActorIds[hitId] = actorId;
            if (!_actorIdToHitIds.TryGetValue(actorId, out var list)) {
                list = _listPool.Get();
                _actorIdToHitIds[actorId] = list;
            }

            list.Add(hitId);
            return hitId;
        }

        /// <inheritdoc/>
        void IWorldCollisionService.UnregisterReceive(int receiveId) {
            if (!_receiveIdToActorIds.TryGetValue(receiveId, out var actorId)) {
                return;
            }

            _hitDetectionEngine.UnregisterReceive(receiveId);
            _hidIdToActorIds.Remove(receiveId);
            _receiveIdToListeners.Remove(receiveId);
            if (_actorIdToReceiveIds.TryGetValue(actorId, out var list)) {
                list.Remove(receiveId);
                if (list.Count == 0) {
                    _listPool.Release(list);
                    _actorIdToReceiveIds.Remove(actorId);
                }
            }
        }

        /// <inheritdoc/>
        void IWorldCollisionService.UnregisterHit(int hitId) {
            if (!_hidIdToActorIds.TryGetValue(hitId, out var actorId)) {
                return;
            }

            _hitDetectionEngine.UnregisterHit(hitId);
            _hidIdToActorIds.Remove(hitId);
            if (_actorIdToHitIds.TryGetValue(actorId, out var list)) {
                list.Remove(hitId);
                if (list.Count == 0) {
                    _listPool.Release(list);
                    _actorIdToHitIds.Remove(actorId);
                }
            }
        }
    }
}