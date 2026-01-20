using System;
using System.Collections.Generic;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// Material管理用のBodyComponent
    /// </summary>
    public class MaterialBodyComponent : BodyComponent {
        /// <summary>
        /// マテリアルグループ情報
        /// </summary>
        [Serializable]
        private sealed class MaterialGroupInfo {
            public string Key;
            public MaterialInfo[] MaterialInfos;
        }

        /// <summary>
        /// マテリアル情報
        /// </summary>
        [Serializable]
        private sealed class MaterialInfo {
            public Renderer Renderer;
            public int MaterialIndex;
        }

        [SerializeField, Tooltip("マテリアルグループ情報リスト")]
        private MaterialGroupInfo[] _materialGroupInfos;

        private Dictionary<string, MaterialGroupInfo> _materialGroupInfoMap = new();

        /// <summary>
        /// 色の設定
        /// </summary>
        public void SetColor(string key, Color val) {
            if (_materialGroupInfoMap.TryGetValue(key, out var groupInfo)) {
                foreach (var info in groupInfo.MaterialInfos) {
                    info.Renderer.materials[info.MaterialIndex].color = val;
                }
            }
        }

        /// <summary>
        /// 色の設定
        /// </summary>
        public void SetColor(string key, int nameId, Color val) {
            if (_materialGroupInfoMap.TryGetValue(key, out var groupInfo)) {
                foreach (var info in groupInfo.MaterialInfos) {
                    info.Renderer.materials[info.MaterialIndex].SetColor(nameId, val);
                }
            }
        }

        /// <summary>
        /// Intの設定
        /// </summary>
        public void SetInt(string key, int nameId, int val) {
            if (_materialGroupInfoMap.TryGetValue(key, out var groupInfo)) {
                foreach (var info in groupInfo.MaterialInfos) {
                    info.Renderer.materials[info.MaterialIndex].SetInt(nameId, val);
                }
            }
        }

        /// <summary>
        /// Floatの設定
        /// </summary>
        public void SetFloat(string key, int nameId, float val) {
            if (_materialGroupInfoMap.TryGetValue(key, out var groupInfo)) {
                foreach (var info in groupInfo.MaterialInfos) {
                    info.Renderer.materials[info.MaterialIndex].SetFloat(nameId, val);
                }
            }
        }

        /// <summary>
        /// Textureの設定
        /// </summary>
        public void SetTexture(string key, int nameId, Texture val) {
            if (_materialGroupInfoMap.TryGetValue(key, out var groupInfo)) {
                foreach (var info in groupInfo.MaterialInfos) {
                    info.Renderer.materials[info.MaterialIndex].SetTexture(nameId, val);
                }
            }
        }

        /// <summary>
        /// 生成時処理
        /// </summary>
        private void Awake() {
            _materialGroupInfoMap.Clear();
            foreach (var groupInfo in _materialGroupInfos) {
                _materialGroupInfoMap[groupInfo.Key] = groupInfo;
            }
        }
    }
}