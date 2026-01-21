using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SampleEngine {
    /// <summary>
    /// アクター用のGameObject操作用クラス
    /// </summary>
    public sealed class Body : IDisposable {
        private readonly Dictionary<Type, BodyComponent> _components = new();
        private readonly List<IBodyComponentRuntime> _componentRuntimes = new();

        private GameObject _gameObject;

        /// <summary>制御対象のGameObject</summary>
        public GameObject GameObject => _gameObject;
        /// <summary>制御対象のTransform</summary>
        public Transform Transform => _gameObject.transform;
        /// <summary>座標</summary>
        public Vector3 Position {
            get => Transform.position;
            set => Transform.position = value;
        }
        /// <summary>姿勢</summary>
        public Quaternion Rotation {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }
        /// <summary>オイラー回転量</summary>
        public Vector3 EulerAngles {
            get => Transform.eulerAngles;
            set => Transform.eulerAngles = value;
        }
        /// <summary>ローカルスケール</summary>
        public Vector3 LocalScale {
            get => Transform.localScale;
            set => Transform.localScale = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="gameObject">生成済みのGameObject</param>
        public Body(GameObject gameObject) {
            _gameObject = gameObject;

            var components = _gameObject.GetComponentsInChildren<BodyComponent>(true);
            foreach (var component in components) {
                _components[component.GetType()] = component;
                _componentRuntimes.Add(component);
            }
        }

        /// <summary>
        /// 廃棄処理
        /// </summary>
        public void Dispose() {
            _components.Clear();
            _componentRuntimes.Clear();
            if (_gameObject != null) {
                Object.Destroy(_gameObject);
                _gameObject = null;
            }
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update(float deltaTime) {
            foreach (var runtime in _componentRuntimes) {
                runtime.Tick(deltaTime);
            }
        }

        /// <summary>
        /// 後更新処理
        /// </summary>
        public void LateUpdate(float deltaTime) {
            foreach (var runtime in _componentRuntimes) {
                runtime.PostTick(deltaTime);
            }
        }

        /// <summary>
        /// BodyComponentの追加
        /// </summary>
        public TComponent AddBodyComponent<TComponent>()
            where TComponent : BodyComponent {
            var type = typeof(TComponent);
            if (_components.ContainsKey(type)) {
                throw new InvalidOperationException($"Component already exists: {type}");
            }

            var component = _gameObject.AddComponent<TComponent>();
            _components[type] = component;
            _componentRuntimes.Add(component);
            return component;
        }

        /// <summary>
        /// Unity用のComponent取得
        /// </summary>
        public TComponent GetComponent<TComponent>() where TComponent : Component => _gameObject.GetComponent<TComponent>();

        /// <summary>
        /// Unity用のComponent追加
        /// </summary>
        public TComponent AddComponent<TComponent>() where TComponent : Component => _gameObject.AddComponent<TComponent>();

        /// <summary>
        /// Unity用のComponent取得
        /// </summary>
        public TComponent[] GetComponents<TComponent>() where TComponent : Component => _gameObject.GetComponents<TComponent>();

        /// <summary>
        /// Unity用のComponent取得
        /// </summary>
        public TComponent[] GetComponentsInChildren<TComponent>() where TComponent : Component => _gameObject.GetComponentsInChildren<TComponent>();

        /// <summary>
        /// Unity用のComponent取得
        /// </summary>
        public TComponent[] GetComponentsInParent<TComponent>() where TComponent : Component => _gameObject.GetComponentsInParent<TComponent>();
    }
}