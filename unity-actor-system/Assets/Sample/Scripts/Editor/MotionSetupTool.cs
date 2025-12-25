using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sample {
    /// <summary>
    /// モーション初期化用ツール
    /// </summary>
    public static class MotionSetupTool {
        /// <summary>
        /// キャラモーションの初期化
        /// </summary>
        public static void SetupCharacterMotion(GameObject asset) {
            var path = AssetDatabase.GetAssetPath(asset);
            var assetName = Path.GetFileNameWithoutExtension(path);
            var importer = (ModelImporter)AssetImporter.GetAtPath(path);

            // ClipAnimationを取得
            var clipAnimations = importer.clipAnimations;
            if (clipAnimations.Length <= 0) {
                clipAnimations = importer.defaultClipAnimations;
            }

            // 初期化処理
            var multiClip = clipAnimations.Length > 1;
            for (var i = 0; i < clipAnimations.Length; i++) {
                var clipAnimation = clipAnimations[i];
                var clipName = multiClip ? $"{assetName}_{i:D2}" : assetName;
                clipAnimation.name = clipName;
                clipAnimation.takeName = clipName;
                if (assetName.EndsWith("_lp")) {
                    clipAnimation.loopTime = true;
                    clipAnimation.loopPose = true;
                }
            }

            importer.clipAnimations = clipAnimations;
            importer.SaveAndReimport();
        }

        /// <summary>
        /// 選択中アセットの初期化
        /// </summary>
        [MenuItem("Assets/Sample/Setup Character Motions")]
        private static void SetupCharacterMotionsForSelection() {
            var assets = Selection.gameObjects;
            try {
                var progress = 0.0f;
                EditorUtility.DisplayProgressBar("Setup Character Motions", "Processing...", progress);
                for (var i = 0; i < assets.Length; i++) {
                    var asset = assets[i];
                    progress = (i + 1) / (float)assets.Length;
                    var type = PrefabUtility.GetPrefabAssetType(asset);
                    if (type != PrefabAssetType.Model) {
                        continue;
                    }

                    SetupCharacterMotion(asset);
                    EditorUtility.DisplayProgressBar("Setup Character Motions", asset.name, progress);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}