using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

using System.Linq;
using UnityEngine.AddressableAssets;

using System.IO;

namespace AssetBundle
{
    public class AssetBundleDownLoader
    {
        public struct AssetBundleDownloadInfo
        {
            public string Label;
            public long Size;
        }

        private long _totalBundleSize = 0;
        private List<AssetBundleDownloadInfo> _downloadList = new List<AssetBundleDownloadInfo>();
        private List<string> _labels = null;
        public long TotalBundleSize { get => _totalBundleSize; }

        private Action<string> _onChangeDownloadBundle = null;
        private Action<float> _onUpdateDownloadProgress = null;

        public AssetBundleDownLoader(AddressableLabels labels)
        {
            _labels = labels.Labels;
        }

        public void SetBindingCallBack(Action<string> onChangeDownloadBundle, Action<float> onUpdateDownloadProgress)
        {
            _onChangeDownloadBundle = onChangeDownloadBundle;
            _onUpdateDownloadProgress = onUpdateDownloadProgress;
        }

        public void Remove()
        {
        }

        public async UniTask<bool> CheckUpdateBundle()
        {
            // 업데이트 목록 체크 
            for (int i = 0; i < _labels.Count; i++)
            {

                await CheckBundleSize(_labels[i]);
            }

            // 체크 완료

            return _totalBundleSize > 0;
        }

        private async UniTask CheckBundleSize(string label)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            while (handle.IsDone == false)
            {
                await UniTask.DelayFrame(1);
            }

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                if (handle.Result > 0)
                    _downloadList.Add(new AssetBundleDownloadInfo() { Label = label, Size = handle.Result });

                _totalBundleSize += handle.Result;
            }

            Addressables.Release(handle);
        }

        public bool CheckDownloadExistBundle()
        {
            if (_totalBundleSize == 0)
                return false;

            return true;
        }

        public async UniTask AssetBundleDownLoad()
        {
            for (int i = 0; i < _downloadList.Count; i++)
            {
                await AssetBundleDownLoad(_downloadList[i].Label, _downloadList[i].Size);
            }

            RemoveOldVersionAssetBundleCaching();
        }

        private async UniTask AssetBundleDownLoad(string label, long size)
        {
            var downHandle = Addressables.DownloadDependenciesAsync(label);

            _onChangeDownloadBundle?.Invoke(label);

            await UniTask.DelayFrame(5);

            while (downHandle.IsDone == false)
            {
                _onUpdateDownloadProgress?.Invoke(downHandle.GetDownloadStatus().Percent);

                await UniTask.DelayFrame(1);
            }
            await UniTask.DelayFrame(5);

            Addressables.Release(downHandle);
        }


        public void RemoveOldVersionAssetBundleCaching()
        {
            var bundleNames = new HashSet<string>();
            var bundles = new HashSet<CachedAssetBundle>();

            // 최신 Asset Bundle 정보를 가져온다.
            foreach (var loc in Addressables.ResourceLocators)
            {
                foreach (var key in loc.Keys)
                {
                    if (loc.Locate(key, typeof(object), out var resourceLocations))
                    {
                        foreach (var l in resourceLocations)
                        {
                            if (l.HasDependencies)
                            {
                                foreach (var d in l.Dependencies)
                                {
                                    var bundleName = Path.GetFileName(d.InternalId);

                                    if (!bundleNames.Contains(bundleName))
                                    {
                                        if (d.Data is AssetBundleRequestOptions dependencyBundle)
                                        {
                                            bundleNames.Add(bundleName);
                                            bundles.Add(new CachedAssetBundle(bundleName, Hash128.Parse(dependencyBundle.Hash)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var cache = Caching.GetCacheAt(0);
            var path = cache.path;

            // 캐싱된 번들 Hash 값과 현재 Hash 값을 비교해서 다른 파일들을 List 에 넣는다.
            var bundlesToRemove = new List<CachedAssetBundle>();
            try
            {
                var folders = Directory.GetDirectories(path);
                foreach (var folder in folders)
                {
                    var folderName = new DirectoryInfo(folder).Name;
                    var hashes = new List<Hash128>();

                    Caching.GetCachedVersions(folderName, hashes);

                    foreach (var ver in hashes)
                    {
                        if (!bundles.Any(x => x.hash == ver))
                        {
                            var cachBundle = new CachedAssetBundle(folderName, ver);
                            bundlesToRemove.Add(cachBundle);
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            // List 에 넣는 파일을 삭제한다.
            foreach (var bundle in bundlesToRemove)
            {
                var bundleName = bundle.name;

                if (Caching.ClearCachedVersion(bundleName, bundle.hash))
                {
                    Debug.Log("Cleared old bundle " + bundleName);
                }
                else
                {
                    Debug.Log("failed to delete bundle " + bundleName);
                }
            }
        }
    }
}
