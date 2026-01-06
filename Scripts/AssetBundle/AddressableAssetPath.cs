#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
using UnityEngine;

namespace AssetBundle
{
    public class AddressableAssetPath
    {
        private const string ASSET_GROUP_PATH = "Assets/AddressableAssetsData/AssetGroups/";

        private static Dictionary<string, string> _cacheAssetPath = null;
        private static Dictionary<string, string> _cacheAssetPathByDirectory = null;

        public static Dictionary<string, string> CacheAssetPathByDirectory => _cacheAssetPathByDirectory;
        public static Dictionary<string, string> CacheAssetPath
        {
            get
            {
                if (_cacheAssetPath == null)
                {
                    _cacheAssetPath = new Dictionary<string, string>();
                    _cacheAssetPathByDirectory = new Dictionary<string, string>();
                    string[] fileEntries = System.IO.Directory.GetFiles(ASSET_GROUP_PATH);
                    foreach (string fileName in fileEntries)
                    {
                        var load = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(fileName);

                        if (load != null)
                        {
                            foreach (var entry in load.entries)
                            {
                                _cacheAssetPath.Add(entry.address, entry.AssetPath);
                                _cacheAssetPathByDirectory.Add(entry.AssetPath, entry.address);
                            }
                        }
                    }
                }

                return _cacheAssetPath;
            }
        }

        public static void Reload()
        {
            if (AssetbundleDuplicateCheck() == true)
                return;

            _cacheAssetPath = new Dictionary<string, string>();
            _cacheAssetPathByDirectory = new Dictionary<string, string>();
            string[] fileEntries = System.IO.Directory.GetFiles(ASSET_GROUP_PATH);
            foreach (string fileName in fileEntries)
            {
                var load = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(fileName);

                if (load != null)
                {
                    foreach (var entry in load.entries)
                    {
                        if (_cacheAssetPath.ContainsKey(entry.address))
                        {
                            Debug.LogError($"Addressable 중복 키 {entry.address}");
                        }
                        else
                        {
                            _cacheAssetPath.Add(entry.address, entry.AssetPath);
                        }

                        
                        if (string.IsNullOrEmpty(entry.AssetPath))
                        {
                            Debug.LogError($"Asset 이 없습니다. {entry.address}");
                        }
                        if (_cacheAssetPathByDirectory.ContainsKey(entry.AssetPath))
                        {
                            Debug.LogError($"AssetPath 중복 키 {entry.AssetPath}");
                        }
                        else
                        {
                            _cacheAssetPathByDirectory.Add(entry.AssetPath, entry.address);
                        }
                       
                    }
                }
            }            
        }

        public static bool AssetbundleDuplicateCheck()
        {
            var address = new List<string>();
            var duplicateList = new List<string>();

            string[] fileEntries = System.IO.Directory.GetFiles(ASSET_GROUP_PATH);
            foreach (string fileName in fileEntries)
            {
                var load = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(fileName);

                if (load != null)
                {
                    foreach (var entry in load.entries)
                    {
                        if(address.IndexOf(entry.address) != -1)
                        {
                            duplicateList.Add(entry.address);
                        }
                        else
                        {
                            address.Add(entry.address);
                        }
                    }
                }
            }

            if(duplicateList.Count > 0)
            {
                string list = "Addressable AssetBundle 중에 중복된 키값이나 에러가 존재합니다.";
            
                foreach(var path in duplicateList)
                {
                    list += path + "\n";
                }
                Debug.LogError($"Addressable AssetBundle Error {list}");
            
                //return true;
            }
            

            return false;
        }
    }
}

#endif