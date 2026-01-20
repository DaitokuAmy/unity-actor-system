using System.Threading;
using Cysharp.Threading.Tasks;
using Sample.Application;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sample.Infrastructure {
    /// <summary>
    /// キャラアセット読み込み管理用ストア
    /// </summary>
    public sealed class CameraAssetStore : ICameraAssetStore {
        private const string RootPath = "Assets/SampleAssets/Actor/Camera/";
        
        /// <summary>
        /// キャラプレファブのロード
        /// </summary>
        async UniTask<GameObject> ICameraAssetStore.LoadCameraPrefabAsync(string key, CancellationToken ct) {
            var handle = Addressables.LoadAssetAsync<GameObject>(GetCameraPrefabPath(key));
            await handle.Task;
            return handle.Result;
        }

        /// <summary>
        /// CameraPrefabのPath
        /// </summary>
        private string GetCameraPrefabPath(string key) {
            var folderName = key.Substring(0, "cam000".Length);
            return $"{RootPath}{folderName}/Data/dat_act_{key}.asset";
        }
    }
}