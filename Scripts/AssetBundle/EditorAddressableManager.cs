using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

namespace AssetBundle
{
    /// <summary>
    /// 에디터 전용 Addressable Manager.
    /// 모든 비동기 로드 요청을 WaitForCompletion()으로 강제 동기화하여
    /// [ContextMenu]나 에디터 스크립트 흐름이 끊기지 않게 합니다.
    /// </summary>
    public class EditorAddressableManager : IAddressableManager
    {
        public AsyncOperationHandle<T> Load<T>(string assetName) where T : UnityEngine.Object
        {
            if (CheckKeyInvalid(assetName)) return default;

            var handle = Addressables.LoadAssetAsync<T>(assetName);

            // [핵심] 에디터 흐름을 위해 강제로 로딩이 끝날 때까지 대기
            handle.WaitForCompletion();

            return handle;
        }

        public void Load<T>(string assetName, Action<T> result) where T : UnityEngine.Object
        {
            if (CheckKeyInvalid(assetName)) return;

            var handle = Addressables.LoadAssetAsync<T>(assetName);

            // 동기 대기
            T loadedAsset = handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                result?.Invoke(loadedAsset);
            }
            else
            {
                Debug.LogError($"[EditorAddressableManager] Load Failed: {assetName}");
            }
        }

        public T LoadWaitForCompletion<T>(string assetName) where T : UnityEngine.Object
        {
            if (CheckKeyInvalid(assetName)) return null;

            var handle = Addressables.LoadAssetAsync<T>(assetName);

            // 원래 기능도 동기 대기이므로 그대로 수행
            return handle.WaitForCompletion();
        }

        public AsyncOperationHandle<GameObject> LoadInstantiate(string assetName, Transform transform)
        {
            if (CheckKeyInvalid(assetName)) return default;

            var handle = Addressables.InstantiateAsync(assetName, transform);
            handle.WaitForCompletion(); // 즉시 생성 보장
            return handle;
        }

        public void LoadInstantiate<T>(string assetName, Transform transform, Action<T> result) where T : UnityEngine.Object
        {
            if (CheckKeyInvalid(assetName)) return;

            var handle = Addressables.InstantiateAsync(assetName, transform);
            GameObject go = handle.WaitForCompletion(); // 즉시 생성

            if (handle.Status == AsyncOperationStatus.Succeeded && go != null)
            {
                T component = go.GetComponent<T>();
                result?.Invoke(component);
            }
            else
            {
                Debug.LogError($"[EditorAddressableManager] Instantiate Failed: {assetName}");
            }
        }

        public GameObject LoadInstantiate(string assetName, Transform transform, bool instantiateInWorldSpace)
        {
            if (CheckKeyInvalid(assetName)) return null;

            var handle = Addressables.InstantiateAsync(assetName, transform, instantiateInWorldSpace);
            return handle.WaitForCompletion();
        }

        public void Release()
        {
            // 에디터에서는 특별한 전체 해제 로직이 필요 없다면 비워둠
            // 필요 시 Resources.UnloadUnusedAssets() 등을 호출
        }

        public void Release<TObject>(TObject asset) where TObject : UnityEngine.Object
        {
            // 에디터라도 Addressables 시스템의 핸들 관리를 위해 호출해주는 것이 좋음
            Addressables.Release(asset);
        }

        public void ReleaseInstantiate(GameObject asset)
        {
            // 생성된 인스턴스 해제
            Addressables.ReleaseInstance(asset);
        }

        public void AllRemoveAssetBundleCaching()
        {
            Caching.ClearCache();
        }

        private bool CheckKeyInvalid(string key)
        {
            if (string.IsNullOrEmpty(key) || key == "NONE")
            {
                Debug.LogWarning($"[EditorAddressableManager] Invalid Key: {key}");
                return true;
            }
            return false;
        }
    }
}