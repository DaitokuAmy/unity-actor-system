using ActionSequencer;
using Sample.Application;
using SampleEngine;
using UnityEngine;

namespace Sample.Presentation {
    /// <summary>
    /// ハンドラ
    /// </summary>
    public sealed class AttackRangeSequenceEventHandler : RangeSequenceEventHandler<AttackRangeSequenceEvent>, ISphereHitCollider {
        private IWorldCollisionService _worldCollisionService;
        private int _actorId;
        private Transform _baseTransform;
        private int _layerMask;
        private Vector3 _offset;
        private float _radius;
        private bool _constraint;
        private int _collisionId;

        /// <inheritdoc/>
        Vector3 ISphereHitCollider.Center => _constraint ? _baseTransform.TransformPoint(_offset) : _offset;
        /// <inheritdoc/>
        float ISphereHitCollider.Radius => _radius;

        /// <inheritdoc/>
        protected override void OnEnter(AttackRangeSequenceEvent sequenceEvent) {
            _constraint = sequenceEvent.Constraint;
            _offset = _constraint ? sequenceEvent.OffsetPositon : _baseTransform.TransformPoint(sequenceEvent.OffsetPositon);
            _radius = sequenceEvent.Radius;
            _collisionId = _worldCollisionService.RegisterHit(_actorId, this, _layerMask, CreateAttackParams());
        }

        /// <inheritdoc/>
        protected override void OnExit(AttackRangeSequenceEvent sequenceEvent) {
            _worldCollisionService.UnregisterHit(_collisionId);
        }

        /// <inheritdoc/>
        protected override void OnCancel(AttackRangeSequenceEvent sequenceEvent) {
            OnExit(sequenceEvent);
        }

        /// <summary>
        /// 情報セットアップ
        /// </summary>
        public void Setup(IWorldCollisionService worldCollisionService, int actorId, Transform baseTransform, int layerMask) {
            _worldCollisionService = worldCollisionService;
            _actorId = actorId;
            _baseTransform = baseTransform;
            _layerMask = layerMask;
        }

        /// <summary>
        /// 攻撃パラメータの生成
        /// </summary>
        private AttackParams CreateAttackParams() {
            return new AttackParams { direction = _baseTransform.forward };
        }
    }
}