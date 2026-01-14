using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using System;
using Utils;

namespace AssetBundle
{
    public class AddressableManager : IAddressableManager
    {
        public AsyncOperationHandle<T> Load<T>(string assetName) where T : UnityEngine.Object
        {
            return Addressables.LoadAssetAsync<T>(assetName);
        }

        public void Load<T>(string assetName, Action<T> result) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetName) == true || assetName == "NONE")
            {
                Debug.LogError(string.Format("AssetBundleManager Error assetName : {0} ", assetName));
                return;
            }

            //Debug.LogFormat("<color=cyan>Drum :: Grm.Addr.Load({0})</color>", assetName);

            Addressables.LoadAssetAsync<T>(assetName).Completed += (AsyncOperationHandle<T> asset) =>
            {
                if (asset.Status == AsyncOperationStatus.Succeeded)
                {
                    result(asset.Result);
                }
                else
                {
                    Debug.Log("AssetBundle Load Fail " + assetName);
                }
            };
        }

        public T LoadWaitForCompletion<T>(string assetName) where T : UnityEngine.Object
        {
            var hanlder = Addressables.LoadAssetAsync<T>(assetName);
            if (hanlder.Status == AsyncOperationStatus.Failed)
                return null;

            return hanlder.WaitForCompletion();
        }

        public AsyncOperationHandle<GameObject> LoadInstantiate(string assetName, Transform transform)
        {
            return Addressables.InstantiateAsync(assetName, transform);
        }

        public void LoadInstantiate<T>(string assetName, Transform transform, Action<T> result) where T : UnityEngine.Object
        {
            Addressables.InstantiateAsync(assetName).Completed += (AsyncOperationHandle<GameObject> asset) =>
            {
                T component = asset.Result.GetComponent<T>();

                if (transform != null)
                    asset.Result.transform.SetParent(transform);

                result(component);
            };
        }

        public GameObject LoadInstantiate(string assetName, Transform transform, bool instantiateInWorldSpace)
        {
            var handle = Addressables.InstantiateAsync(assetName, transform, instantiateInWorldSpace);
            if (handle.Status == AsyncOperationStatus.Failed)
                return null;

            return handle.WaitForCompletion();
        }

        public void Release()
        {

        }

        public void Release<TObject>(TObject asset) where TObject : UnityEngine.Object
        {
            Addressables.Release(asset);
        }

        public void ReleaseInstantiate(GameObject asset)
        {
            Addressables.ReleaseInstance(asset);
        }


        public void AllRemoveAssetBundleCaching()
        {
            Caching.ClearCache();
        }
    }
}
