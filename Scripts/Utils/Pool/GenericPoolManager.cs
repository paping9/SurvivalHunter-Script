using AssetBundle;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using VContainer;

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
        // 비동기 로딩 중인 작업들을 추적하기 위한 딕셔너리 (Key: PoolKey, Value: Loading Task)
        private readonly Dictionary<string, UniTask> _loadingOps = new();

        private readonly Transform _rootContainer;
        private readonly IAddressableManager _addressableManager;
        private readonly IObjectResolver _resolver;

        // 생성자 주입을 통해 AddressableManager를 받습니다.
        public GenericPoolManager(IObjectResolver resolver, IAddressableManager addressableManager)
        {
            _resolver = resolver;
            _addressableManager = addressableManager;
            _rootContainer = new GameObject("GenericPoolManager").transform;
            GameObject.DontDestroyOnLoad(_rootContainer.gameObject); // 필요 시 추가
        }

        public async UniTask<T> Get<T>(string key, int count = 30) where T : Component
        {
            // 1. 이미 풀이 생성되어 있다면 즉시 반환
            if (_poolMap.TryGetValue(key, out var poolObj))
            {
                return (poolObj as GenericPool<T>)?.Get();
            }

            // 2. 현재 해당 키로 로딩(풀 생성)이 진행 중인지 확인
            if (_loadingOps.TryGetValue(key, out var loadingTask))
            {
                // 로딩 중이라면 끝날 때까지 기다림 (중복 로딩 방지)
                await loadingTask;

                // 로딩이 끝났으므로 풀이 존재해야 함
                if (_poolMap.TryGetValue(key, out var loadedPool))
                {
                    return (loadedPool as GenericPool<T>)?.Get();
                }

                throw new Exception($"[GenericPoolManager] Loading finished but pool not found for key: {key}");
            }

            // 3. 로딩 시작 및 작업 등록 (동시성 제어)
            var task = LoadAndCreatePoolAsync<T>(key, count);
            _loadingOps[key] = task;

            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GenericPoolManager] Failed to load pool for key '{key}': {e}");
                throw;
            }
            finally
            {
                // 로딩 완료(성공이든 실패든) 후 작업 목록에서 제거
                _loadingOps.Remove(key);
            }

            // 4. 생성 완료된 풀에서 아이템 반환
            return ((GenericPool<T>)_poolMap[key]).Get();
        }

        // 실제 로딩 및 풀 생성을 담당하는 내부 메서드
        private async UniTask LoadAndCreatePoolAsync<T>(string key, int count) where T : Component
        {
            // AddressableManager를 주입받은 인스턴스로 사용
            var prefab = await _addressableManager.Load<T>(key);

            if (prefab == null)
            {
                throw new Exception($"[GenericPoolManager] Failed to load prefab addressable: {key}");
            }

            var go = new GameObject($"GameObjectPool_{prefab.name}");
            go.transform.SetParent(_rootContainer);

            var genericPool = new GameObjectPoolStrategy<T>(_resolver, key, prefab.GetComponent<T>(), go.transform);

            // GetOrCreate 내부 로직을 재사용하되, 중복 체크는 이미 상위에서 수행됨
            CreateAndRegisterPool(key, genericPool, count);
        }

        public T GetClass<T>(string key, int count = 30) where T : class, new()
        {
            if (_poolMap.TryGetValue(key, out var pool))
            {
                return (pool as GenericPool<T>)?.Get();
            }

            // 클래스 풀은 비동기 로딩이 없으므로 바로 생성
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

            return CreateAndRegisterPool(key, strategy, count);
        }

        // 중복 코드를 줄이기 위한 내부 헬퍼 메서드
        private GenericPool<T> CreateAndRegisterPool<T>(string key, IGenericPoolStrategy<T> strategy, int count) where T : class
        {
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
                // 풀이 없으면 경고 후 파괴 (혹은 예외 처리)
                Debug.LogWarning($"[PoolManager] Trying to release item to non-existent pool: {key}");

                if (item is Component comp)
                {
                    if (Application.isPlaying)
                        UnityEngine.Object.Destroy(comp.gameObject);
                    else
                        UnityEngine.Object.DestroyImmediate(comp.gameObject);
                }
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
            _loadingOps.Clear(); // 로딩 중인 작업 정보도 초기화
        }
    }
}