using System;
using System.Collections.Generic;
using AssetBundle;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Extension;

namespace Utils.Pool
{
    public enum PopOptionForNotEnough
    {
        None,       // Default: maxCount 넘으면 null 반환
        ForceReuse, // activeList 중 하나를 강제로 꺼냄 (가장 오래된 것)
        ForceCreate // maxCount 넘더라도 새로 생성함
    }
    
    public class GenericPoolManager : IGenericPoolManager
    {
        private readonly Dictionary<string, object> _poolMap = new();
        private Transform _rootContainer;

        public GenericPoolManager()
        {
            _rootContainer = new GameObject("GenericPoolManager").transform;
        }
        
        public async UniTask<T> Get<T>(string key, int count = 30) where T : Component
        {
            if(_poolMap.TryGetValue(key, out var pool))
            {
                return (pool as GenericPool<T>)?.Get();
            }

            var prefab = await SystemLocator.Get<AddressableManager>().Load<T>(key);
            var go = new GameObject($"GameObjectPool_{prefab.name}");
            var genericPool = new GameObjectPoolStrategy<T>(key, prefab.GetComponent<T>(), go.transform);
            go.transform.SetParent(_rootContainer);
            
            return GetOrCreate(key,  genericPool, count).Get();
        }

        public T GetClass<T>(string key, int count = 30) where T : class, new()
        {
            if(_poolMap.TryGetValue(key, out var pool))
            {
                return (pool as GenericPool<T>)?.Get();
            }
            
            return GetOrCreate(key, new ClassPoolStrategy<T>(), count).Get();
        }
        
        public GenericPool<T> GetOrCreate<T>(string key, IGenericPoolStrategy<T> strategy, int count = 30) where T : class
        {
            if (_poolMap.TryGetValue(key, out var poolObj))
            {
                if (poolObj is GenericPool<T> typedPool)
                    return typedPool;

                throw new InvalidOperationException($"[PoolManager] Key '{key}' is already registered with a different type.");
            }

            var newPool = new GenericPool<T>(strategy, count);
            _poolMap.Add(key, newPool);
            return newPool;
        }

        public void Release<T>(string key, T item) where T : class
        {
            if (_poolMap.TryGetValue(key, out var poolObj) && poolObj is GenericPool<T> pool)
            {
                pool.Release(item);
            }
            else
            {
                throw new KeyNotFoundException($"[PoolManager] No pool found for key: {key}");
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    if (item is Component comp)
                    {
                        GameObject.DestroyImmediate(comp.gameObject);
                    }
                }
#endif
            }
        }

        public void Clear(string key)
        {
            if (_poolMap.TryGetValue(key, out var poolObj) && poolObj is IPoolClearable clearable)
            {
                clearable.Clear();
                _poolMap.Remove(key);
            }
        }

        public void ClearAll()
        {
            foreach (var poolObj in _poolMap.Values)
            {
                if (poolObj is IPoolClearable clearable)
                    clearable.Clear();
            }

            _poolMap.Clear();
        }
    }
}