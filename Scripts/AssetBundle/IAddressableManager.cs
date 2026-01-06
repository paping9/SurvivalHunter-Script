using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetBundle
{
    public interface IAddressableManager
    {
        AsyncOperationHandle<T> Load<T>(string assetName) where T : UnityEngine.Object;
        T LoadWaitForCompletion<T>(string assetName) where T : UnityEngine.Object;
        AsyncOperationHandle<GameObject> LoadInstantiate(string assetName, Transform transform);
        void LoadInstantiate<T>(string assetName, Transform transform, Action<T> result) where T : UnityEngine.Object;
        GameObject LoadInstantiate(string assetName, Transform transform, bool instantiateInWorldSpace);
        void Release();
        void Release<TObject>(TObject asset) where TObject : UnityEngine.Object;
        void ReleaseInstantiate(GameObject asset);
        void AllRemoveAssetBundleCaching();
    }
}