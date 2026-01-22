using System.Collections.Generic;
using Sample.Application;
using SampleEngine;
using UnityEngine.Pool;

namespace Sample.Infrastructure {
    /// <summary>
    /// 世界のコリジョン管理を提供するサービス
    /// </summary>
    public sealed class WorldCollisionService : IWorldCollisionService, ICollisionListener {
        private readonly CollisionManager _collisionManager;
        private readonly Dictionary<int, int> _collisionIdToActorIds = new();
        private readonly Dictionary<int, List<int>> _actorIdToHitCollisionIds = new();
        private readonly Dictionary<int, List<int>> _actorIdToReceiveCollisionIds = new();
        private readonly Dictionary<int, IWorldCollisionListener> _collisionIdToListeners = new();
        private readonly ObjectPool<List<int>> _listPool;

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionEnter(in CollisionEvent evt) {
            if (!_collisionIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_collisionIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_collisionIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionEnter(hitActorId, receiveActorId, evt.contactPoint);
        }

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionStay(in CollisionEvent evt) {
            if (!_collisionIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_collisionIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_collisionIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionStay(hitActorId, receiveActorId, evt.contactPoint);
        }

        /// <inheritdoc/>
        void ICollisionListener.OnCollisionExit(in CollisionEvent evt) {
            if (!_collisionIdToActorIds.TryGetValue(evt.hitId, out var hitActorId)) {
                return;
            }

            if (!_collisionIdToActorIds.TryGetValue(evt.receiveId, out var receiveActorId)) {
                return;
            }

            if (!_collisionIdToListeners.TryGetValue(evt.receiveId, out var listener)) {
                return;
            }

            listener?.OnCollisionExit(hitActorId, receiveActorId);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorldCollisionService() {
            _listPool = new ObjectPool<List<int>>(() => new List<int>(), list => list.Clear(), list => list.Clear());
            _collisionManager = new CollisionManager(5.0f);
        }

        /// <inheritdoc/>
        void IWorldCollisionService.Update(float deltaTime) {
            _collisionManager.Update();
        }

        /// <inheritdoc/>
        int IWorldCollisionService.RegisterReceive(int actorId, IReceiveCollider collider, int layerMask, IWorldCollisionListener listener) {
            var collisionId = _collisionManager.RegisterReceive(collider, this, layerMask);
            _collisionIdToActorIds[collisionId] = actorId;
            _collisionIdToListeners[collisionId] = listener;
            if (!_actorIdToReceiveCollisionIds.TryGetValue(actorId, out var list)) {
                list = _listPool.Get();
                _actorIdToReceiveCollisionIds[actorId] = list;
            }

            list.Add(collisionId);
            return collisionId;
        }

        /// <inheritdoc/>
        int IWorldCollisionService.RegisterHit(int actorId, IHitCollider collider, int layerMask) {
            var collisionId = _collisionManager.RegisterHit(collider, layerMask);
            _collisionIdToActorIds[collisionId] = actorId;
            if (!_actorIdToHitCollisionIds.TryGetValue(actorId, out var list)) {
                list = _listPool.Get();
                _actorIdToHitCollisionIds[actorId] = list;
            }

            list.Add(collisionId);
            return collisionId;
        }

        /// <inheritdoc/>
        void IWorldCollisionService.UnregisterReceive(int collisionId) {
            if (!_collisionIdToActorIds.TryGetValue(collisionId, out var actorId)) {
                return;
            }

            _collisionManager.UnregisterReceive(collisionId);
            _collisionIdToActorIds.Remove(collisionId);
            _collisionIdToListeners.Remove(collisionId);
            if (_actorIdToReceiveCollisionIds.TryGetValue(actorId, out var list)) {
                list.Remove(collisionId);
                if (list.Count == 0) {
                    _listPool.Release(list);
                    _actorIdToReceiveCollisionIds.Remove(actorId);
                }
            }
        }

        /// <inheritdoc/>
        void IWorldCollisionService.UnregisterHit(int collisionId) {
            if (!_collisionIdToActorIds.TryGetValue(collisionId, out var actorId)) {
                return;
            }

            _collisionManager.UnregisterHit(collisionId);
            _collisionIdToActorIds.Remove(collisionId);
            if (_actorIdToHitCollisionIds.TryGetValue(actorId, out var list)) {
                list.Remove(collisionId);
                if (list.Count == 0) {
                    _listPool.Release(list);
                    _actorIdToHitCollisionIds.Remove(actorId);
                }
            }
        }
    }
}