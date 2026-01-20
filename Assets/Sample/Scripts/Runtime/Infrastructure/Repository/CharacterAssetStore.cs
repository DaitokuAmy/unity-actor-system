using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using SampleEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sample.Infrastructure {
    /// <summary>
    /// キャラアセット読み込み管理用ストア
    /// </summary>
    public sealed class CharacterAssetStore : ICharacterAssetStore {
        private const string RootPath = "Assets/SampleAssets/Actor/Character/";
        
        /// <summary>
        /// キャラプレファブのロード
        /// </summary>
        async UniTask<GameObject> ICharacterAssetStore.LoadCharacterPrefabAsync(string key, CancellationToken ct) {
            var handle = Addressables.LoadAssetAsync<GameObject>(GetCharacterPrefabPath(key));
            await handle.Task;
            return handle.Result;
        }
        
        /// <summary>
        /// キャラアクターデータのロード
        /// </summary>
        async UniTask<CharacterActorData> ICharacterAssetStore.LoadCharacterActorDataAsync(string key, CancellationToken ct) {
            var handle = Addressables.LoadAssetAsync<CharacterActorData>(GetCharacterActorDataPath(key));
            await handle.Task;
            return handle.Result;
        }

        /// <summary>
        /// Character用のPrefabのPath
        /// </summary>
        private string GetCharacterPrefabPath(string key) {
            var folderName = key.Substring(0, "ch000".Length);
            return $"{RootPath}{folderName}/Prefabs/pfb_act_{key}.prefab";
        }

        /// <summary>
        /// CharacterActorDataのPath
        /// </summary>
        private string GetCharacterActorDataPath(string key) {
            var folderName = key.Substring(0, "ch000".Length);
            return $"{RootPath}{folderName}/Data/dat_act_{key}.asset";
        }
    }
}