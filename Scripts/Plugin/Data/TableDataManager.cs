using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Data.Table;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Data
{
    public class CacheTable<T> : ICacheTable<T> where T : TableRaw
    {
        public string TableName { get; private set; }
        public Dictionary<int, T> Data { get; private set; } = new();
    }
    
    public class TableDataManager : ITableDataManager
    {
        protected const string TableDataJsonKey = "TableData/TableData";
        protected readonly Dictionary<string, object> _tableCache = new();
        protected Dictionary<string, List<string>> _tableHierarchy = new();
        
        public async UniTask InitializeAsync()
        {
            await LoadTableDataJson();
            await LoadAllTables();
        }
        
        protected async UniTask LoadTableDataJson()
        {
            var handle = Addressables.LoadAssetAsync<TextAsset>(TableDataJsonKey);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load TableData.json from Addressable: {TableDataJsonKey}");
                return;
            }

            string json = handle.Result.text;
            _tableHierarchy = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            Debug.Log($"✅ Loaded TableData.json: {_tableHierarchy.Count} table groups found.");
        }
        
        protected async UniTask LoadAllTables()
        {
            var tasks = new List<UniTask>();
            
            foreach (var entry in _tableHierarchy)
            {
                foreach (string path in entry.Value)
                {
                    var type = Type.GetType(entry.Key);

                    if (type != null)
                    {
                        MethodInfo method = typeof(TableDataManager)
                            .GetMethod(nameof(LoadTableAsync), BindingFlags.Instance | BindingFlags.Public)
                            ?.MakeGenericMethod(type);

                        if (method != null)
                        {
                            var task = (UniTask)method.Invoke(this, new object[] { path });
                            tasks.Add(task);
                        }
                    }
                }
            }
            
            await UniTask.WhenAll(tasks);
        }
        
        public async UniTask<CacheTable<T>> LoadTableAsync<T>(string path) where T : TableRaw
        {
            string key = typeof(T).Name;

            if (!_tableCache.ContainsKey(key))
            {
                Type cacheTableType = typeof(CacheTable<>).MakeGenericType(typeof(T));
                object cacheTableInstance = Activator.CreateInstance(cacheTableType);
                _tableCache[key] = cacheTableInstance;
            }

            var handle = Addressables.LoadAssetAsync<BaseTableData<T>>(path);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var tableInstance = handle.Result;
                var cacheTable = _tableCache[key] as CacheTable<T>;

                foreach (var data in tableInstance.GetDataArray())
                {
                    cacheTable.Data[data.ID] = data;
                }

                Debug.Log($"✅ Loaded Table: {key} with {cacheTable.Data.Count} entries.");
                return cacheTable;
            }

            Debug.LogError($"❌ Failed to load table: {key}");
            return null;
        }

        public T[] GetTable<T>() where T : TableRaw
        {
            string key = typeof(T).Name;

            if (_tableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                    return datatable.Data.Values.ToArray();
            }

            return null;
        }

        public T Get<T>(int id) where T : TableRaw
        {
            string key = typeof(T).Name;

            if (_tableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                {
                    if (datatable.Data.TryGetValue(id, out var data)) return data;
                }
            }

            return null;
        }
        
        public bool TryGet<T>(int id, out T data) where T : TableRaw
        {
            string key = typeof(T).Name;
            if (_tableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                {
                    return datatable.Data.TryGetValue(id, out data);
                }
            }
            data = null;
            return false;
        }

        public void Clear()
        {
        }
    }
}