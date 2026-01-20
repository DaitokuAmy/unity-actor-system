using System.Threading;
using Cysharp.Threading.Tasks;
using SampleEngine;
using UnityEngine;

namespace Sample.Application {
    /// <summary>
    /// キャラアセット読み込み管理用ストア用インターフェース
    /// </summary>
    public interface ICharacterAssetStore {
        /// <summary>
        /// キャラプレファブのロード
        /// </summary>
        UniTask<GameObject> LoadCharacterPrefabAsync(string key, CancellationToken ct);
        
        /// <summary>
        /// キャラアクターデータのロード
        /// </summary>
        UniTask<CharacterActorData> LoadCharacterActorDataAsync(string key, CancellationToken ct);
    }
}