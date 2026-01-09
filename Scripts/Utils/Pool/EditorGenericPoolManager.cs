using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; // Addressables 필수
using Cysharp.Threading.Tasks;       // UniTask 필수
using VContainer;

namespace Utils.Pool
{
    /// <summary>
    /// 에디터(MapTool) 환경에서 동작하는 가짜 풀 매니저.
    /// 실제 풀링(재사용)은 하지 않고, 생성(Instantiate)과 파괴(DestroyImmediate)만 담당합니다.
    /// </summary>
    public class EditorGenericPoolManager : IGenericPoolManager
    {
        // 로드된 프리팹을 캐싱하여 IO 낭비 방지 (Key: Addressable Key, Value: Prefab)
        private readonly Dictionary<string, GameObject> _prefabCache = new();

        private readonly IObjectResolver _resolver;
        private Transform _rootContainer;

        [Inject]
        public EditorGenericPoolManager(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        // 1. 에디터용 컨테이너 확보 (없으면 생성)
        private Transform GetRootContainer()
        {
            if (_rootContainer == null)
            {
                // 씬에 지저분하게 쌓이지 않도록 임시 폴더 생성
                var go = GameObject.Find("Editor_Pool_Temp");
                if (go == null)
                {
                    go = new GameObject("Editor_Pool_Temp");
                    // 저장되지 않도록 설정 (선택 사항)
                    // go.hideFlags = HideFlags.DontSave; 
                }
                _rootContainer = go.transform;
            }
            return _rootContainer;
        }

        public async UniTask<T> Get<T>(string key, int count = 30) where T : Component
        {
            // 에디터 함수는 동기(Sync)로 돌아가야 안전하므로, 
            // 내부적으로는 동기로 처리하고 결과만 Task로 포장해서 반환합니다.

            // 1. 프리팹 로드 (캐시 확인)
            if (!_prefabCache.TryGetValue(key, out var prefab))
            {
                // [중요] WaitForCompletion()을 사용하여 에디터를 멈추고 즉시 로딩합니다.
                // 이렇게 해야 [ContextMenu] 함수 내에서 순서대로 생성이 보장됩니다.
                var handle = Addressables.LoadAssetAsync<GameObject>(key);
                prefab = handle.WaitForCompletion();

                if (prefab == null)
                {
                    throw new Exception($"[EditorGenericPoolManager] Addressable 로드 실패: {key}");
                }

                _prefabCache[key] = prefab;
            }

            // 2. 생성 (Instantiate)
            // 에디터이므로 풀링 전략(Strategy) 없이 바로 생성합니다.
            var instanceGo = UnityEngine.Object.Instantiate(prefab, GetRootContainer());
            instanceGo.name = $"{prefab.name}_{Guid.NewGuid().ToString().Substring(0, 4)}"; // 구분 쉽게 이름 변경

            // 3. 컴포넌트 가져오기
            var component = instanceGo.GetComponent<T>();
            if (component == null)
            {
                UnityEngine.Object.DestroyImmediate(instanceGo);
                throw new Exception($"[EditorGenericPoolManager] 프리팹에 컴포넌트 {typeof(T).Name} 가 없습니다: {key}");
            }

            // 4. PoolingComponent가 있다면 셋업 (Release 호출 시 키가 필요하므로)
            var pooling = instanceGo.GetComponent<PoolingComponent>();
            if (pooling == null)
            {
                pooling = instanceGo.AddComponent<PoolingComponent<T>>();
            }

            // 수동 주입 (PoolingComponent에 this(Manager)를 넣어줌)
            _resolver.Inject(pooling);

            // PoolingComponent<T>의 Setup 메서드가 있다면 호출 (Reflection이나 인터페이스 활용 권장)
            // 여기서는 구체적인 타입 캐스팅으로 처리 예시:
            if (pooling is PoolingComponent<T> typedPooling)
            {
                typedPooling.Setup(key);
            }

            // Async 인터페이스 규격을 맞추기 위해 완료된 Task 반환
            return component;
        }

        public T GetClass<T>(string key, int count = 30) where T : class, new()
        {
            // 순수 클래스 풀은 에디터에서 그냥 new로 생성
            return new T();
        }

        public GenericPool<T> GetOrCreate<T>(string key, IGenericPoolStrategy<T> strategy, int count = 30) where T : class
        {
            // 에디터에서는 명시적 풀 생성 기능을 지원하지 않거나, 
            // 필요하다면 더미 풀을 리턴할 수 있습니다. 여기선 예외 처리.
            throw new NotImplementedException("[EditorGenericPoolManager] 에디터에서는 GetOrCreate를 지원하지 않습니다. Get을 사용하세요.");
        }

        public void Release<T>(string key, T item) where T : class
        {
            if (item is Component component)
            {
                if (component != null && component.gameObject != null)
                {
                    // 에디터는 즉시 파괴
                    UnityEngine.Object.DestroyImmediate(component.gameObject);
                }
            }
            else
            {
                // C# 객체는 그냥 버림 (GC가 처리)
            }
        }

        public void Clear(string key)
        {
            // 캐시된 프리팹 정도만 날려줍니다.
            if (_prefabCache.ContainsKey(key))
            {
                _prefabCache.Remove(key);
            }
        }

        public void ClearAll()
        {
            _prefabCache.Clear();

            // 임시 컨테이너 청소
            if (_rootContainer != null)
            {
                UnityEngine.Object.DestroyImmediate(_rootContainer.gameObject);
                _rootContainer = null;
            }
        }
    }
}