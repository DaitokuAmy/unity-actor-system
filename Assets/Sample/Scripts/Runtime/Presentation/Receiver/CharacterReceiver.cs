using System;
using R3;
using Sample.Application;
using Sample.Core;
using SampleEngine;
using UnityActorSystem;
using UnityEngine;
using VContainer;

namespace Sample.Presentation {
    /// <summary>
    /// キャライベント監視用クラス
    /// </summary>
    public class CharacterReceiver : IActorReceiver<int>, IWorldCollisionListener {
        [Inject]
        private IWorldCollisionService _worldCollisionService;
        [Inject]
        private ICharacterDamageInputPort _damageInputPort;

        private CompositeDisposable _compositeDisposable;
        private int _receiveCollisionId;

        /// <summary>オーナーアクター</summary>
        protected Actor<int> Owner { get; private set; }
        /// <summary>モデル</summary>
        protected IReadOnlyCharacterModel Model { get; private set; }
        /// <summary>制御用のビュー</summary>
        protected CharacterActorView ActorView { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharacterReceiver(IReadOnlyCharacterModel model, CharacterActorView actorView) {
            Model = model;
            ActorView = actorView;
        }

        /// <inheritdoc/>
        void IDisposable.Dispose() { }

        /// <inheritdoc/>
        void IActorInterface<int>.Activate() {
            _compositeDisposable = new CompositeDisposable();

            // 受けコリジョン登録
            var receiveLayerMask = GetCollisionLayerMask(Model);
            var attackLayerMask = ~receiveLayerMask;
            _receiveCollisionId = _worldCollisionService.RegisterReceive(Owner.Id, ActorView, receiveLayerMask, this);

            var sequenceController = ActorView.SequenceController;
            sequenceController.BindRangeEventHandler<LogRangeSequenceEvent, LogRangeSequenceEventHandler>()
                .AddTo(_compositeDisposable);
            sequenceController.BindRangeEventHandler<CombableRangeSequenceEvent, CombableRangeSequenceEventHandler>(onInit: handler => {
                    handler.Setup(Owner);
                })
                .AddTo(_compositeDisposable);
            sequenceController.BindRangeEventHandler<AttackRangeSequenceEvent, AttackRangeSequenceEventHandler>(onInit: handler => {
                    handler.Setup(_worldCollisionService, Owner.Id, ActorView.Body.Transform, attackLayerMask);
                })
                .AddTo(_compositeDisposable);
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Deactivate() {
            // 受けコリジョン登録解除
            _worldCollisionService.UnregisterReceive(_receiveCollisionId);

            _compositeDisposable.Dispose();
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Attached(Actor<int> actor) {
            Owner = actor;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Detached() {
            Owner = null;
        }

        /// <inheritdoc/>
        void IActorInterface<int>.Update(float deltaTime) { }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionEnter(int hitActorId, int receiveActorId, Vector3 contactPoint, Vector3 contactNormal, object customData) {
            Debug.Log($"OnCollisionEnter: {hitActorId} -> {receiveActorId} ({contactPoint}, {contactNormal})");
            
            // 攻撃パラメータを取得
            if (customData is AttackParams attackParams) {
                // 攻撃ヒット処理を実行
                _damageInputPort.HitAttack(hitActorId, receiveActorId, contactPoint, contactNormal, attackParams);
            }
            
            // シグナルとして通知
            //Owner.CreateSignal<>()
        }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionStay(int hitActorId, int receiveActorId, Vector3 contactPoint, Vector3 contactNormal, object customData) {
            Debug.Log($"OnCollisionStay: {hitActorId} -> {receiveActorId} ({contactPoint}, {contactNormal})");
        }

        /// <inheritdoc/>
        void IWorldCollisionListener.OnCollisionExit(int hitActorId, int receiveActorId, object customData) {
            Debug.Log($"OnCollisionExit: {hitActorId} -> {receiveActorId}");
        }

        /// <summary>
        /// コリジョンレイヤーマスクの取得
        /// </summary>
        private int GetCollisionLayerMask<TModel>()
            where TModel : IReadOnlyCharacterModel {
            var type = typeof(TModel);
            if (typeof(IReadOnlyPlayerModel).IsAssignableFrom(type)) {
                return 1 << 0;
            }

            if (typeof(IReadOnlyEnemyModel).IsAssignableFrom(type)) {
                return 1 << 1;
            }

            return ~0;
        }

        /// <summary>
        /// コリジョンレイヤーマスクの取得
        /// </summary>
        private int GetCollisionLayerMask(IReadOnlyCharacterModel model) {
            if (model is IReadOnlyPlayerModel) {
                return GetCollisionLayerMask<IReadOnlyPlayerModel>();
            }

            if (model is IReadOnlyEnemyModel) {
                return GetCollisionLayerMask<IReadOnlyEnemyModel>();
            }

            return ~0;
        }
    }
}