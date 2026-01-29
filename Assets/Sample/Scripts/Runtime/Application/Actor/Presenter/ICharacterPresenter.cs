using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Core;
using UnityActorSystem;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラ見た目反映用インターフェース
    /// </summary>
    public interface ICharacterPresenter : IActorTransform, IActorPresenter {
        /// <summary>正面方向の角度</summary>
        float ForwardAngleY { get; }
        
        /// <summary>
        /// 待機状態に変更
        /// </summary>
        void ChangeIdle();
        
        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="x">移動量</param>
        /// <param name="y">移動量</param>
        void Move(float x, float y);
        
        /// <summary>
        /// 正面の設定
        /// </summary>
        /// <param name="angleY">正面の設定</param>
        void SetForward(float angleY);

        /// <summary>
        /// 攻撃アクション再生
        /// </summary>
        /// <param name="index">攻撃Index</param>
        /// <param name="ct">キャンセル用トークン</param>
        UniTask PlayAttackActionAsync(int index, CancellationToken ct);

        /// <summary>
        /// ジャンプアクション再生
        /// </summary>
        /// <param name="ct">キャンセル用トークン</param>
        UniTask PlayJumpActionAsync(CancellationToken ct);
        
        /// <summary>
        /// ノックバックアクション再生
        /// </summary>
        /// <param name="damageDirection">ダメージ向き</param>
        /// <param name="ct">キャンセル用トークン</param>
        UniTask PlayKnockbackActionAsync(Vector3 damageDirection, CancellationToken ct);
    }
}