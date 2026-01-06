using System;
using UnityEngine;
using Utils.Extension;

namespace Utils.Pool
{
    public class GameObjectPoolStrategy<T> : IGenericPoolStrategy<T> where T : Component
    {
        private readonly T _prefab;
        private readonly string _poolingKey;
        private readonly Transform _rootContainer;
        
        public GameObjectPoolStrategy(string poolingKey, T prefab, Transform parent)
        {
            _prefab = prefab;
            _poolingKey = poolingKey;
            _rootContainer = parent;
            
            _prefab.transform.SetParent(_rootContainer);
            UnityComponentEx.SetActive(_prefab.gameObject, false);
        }
        
        public T Create()
        {
            var go = GameObject.Instantiate(_prefab.gameObject);
            var component = go.GetComponent<T>();

            var pooling = go.GetOrAddComponent<PoolingComponent<T>>();

            pooling.Setup(_poolingKey);

            return component;
        }

        public void OnGet(T item)
        {
            item.gameObject.SetActive(true);
        }

        public void OnRelease(T item)
        {
            item.gameObject.SetActive(false);
            _prefab.transform.SetParent(_rootContainer);
        }

        public void OnDestroy(T item) => GameObject.Destroy(item.gameObject);

        public void OnReuse(T item)
        {
            item.gameObject.SetActive(true);
        }
    }
}