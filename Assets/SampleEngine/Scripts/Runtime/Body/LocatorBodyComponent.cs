using System;
using System.Collections.Generic;
using UnityEngine;

namespace SampleEngine {
    /// <summary>
    /// LocatorのBodyComponent
    /// </summary>
    public class LocatorBodyComponent : BodyComponent {
        /// <summary>
        /// ロケーター情報
        /// </summary>
        [Serializable]
        private sealed class LocatorInfo {
            public string Key;
            public Transform Target;
        }

        [SerializeField, Tooltip("ロケーター情報リスト")]
        private LocatorInfo[] _locatorInfos;

        private Dictionary<string, LocatorInfo> _locatorInfoMap = new();

        /// <summary>
        /// Locatorの検索
        /// </summary>
        public Transform Find(string key) {
            _locatorInfoMap.TryGetValue(key, out var info);
            return info?.Target;
        }

        /// <summary>
        /// 生成時処理
        /// </summary>
        private void Awake() {
            _locatorInfoMap.Clear();
            foreach (var info in _locatorInfos) {
                _locatorInfoMap[info.Key] = info;
            }
        }
    }
}