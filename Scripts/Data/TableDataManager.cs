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
using Utils;

#if UNITY_EDITOR
using AssetBundle;
using UnityEditor;
#endif

namespace Data
{
    public interface ICacheTable<T> where T : TableRaw
    {
        string TableName { get; }
        Dictionary<int, T> Data { get; }
    }

    public class CacheTable<T> : ICacheTable<T> where T : TableRaw
    {
        public string TableName { get; private set; }
        public Dictionary<int, T> Data { get; private set; } = new();
#if UNITY_EDITOR
        public T GetData(int id)
        {
            if (Data.TryGetValue(id, out var data)) return data;
            return null;
        }

        public void AddData(TableRaw data)
        {
            if (Data.ContainsKey(data.ID)) return;
            Data.Add(data.ID, data as T);
        }

        public void RemoveData(int id)
        {
            if (Data.ContainsKey(id) == false) return;
            Data.Remove(id);
        }
#endif
    }
    
    public class TableDataManager : Singleton<TableDataManager>
    {
        private const string TableDataJsonKey = "TableData/TableData";
        
        private readonly Dictionary<string, object> _tableCache = new();
        private Dictionary<string, List<string>> _tableHierarchy = new();
        
        public async UniTask InitializeAsync()
        {
            await LoadTableDataJson();
            await LoadAllTables();
        }
        
        private async UniTask LoadTableDataJson()
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
        
        /// <summary>
        /// TableData.json을 기반으로 모든 테이블을 Addressable에서 로드
        /// </summary>
        private async UniTask LoadAllTables()
        {
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
                            await (UniTask)method.Invoke(this, new object[] { path });
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Addressable에서 특정 테이블 로드
        /// </summary>
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
        
        
#if UNITY_EDITOR
        public void InitializedEditor()
        {
            string jsonPath = AddressableAssetPath.CacheAssetPath[TableDataJsonKey];
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);

            if (jsonAsset == null)
            {
                Debug.LogError($"❌ Failed to load TableData.json at {jsonPath}");
                return;
            }

            string json = jsonAsset.text;
            _tableHierarchy = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            Debug.Log($"✅ [Editor] Loaded TableData.json: {_tableHierarchy.Count} table groups found.");

            LoadAllTablesEditor();
        }
        private void LoadAllTablesEditor()
        {
            foreach (var entry in _tableHierarchy)
            {
                foreach (string path in entry.Value)
                {
                    var type = Type.GetType(entry.Key);

                    if (type != null)
                    {
                        MethodInfo method = typeof(TableDataManager)
                            .GetMethod(nameof(LoadTableEditor), BindingFlags.Instance | BindingFlags.Public)
                            ?.MakeGenericMethod(type);

                        if (method != null)
                        {
                            method.Invoke(this, new object[] { AddressableAssetPath.CacheAssetPath[path] });
                        }
                    }
                }
            }
        }

        public void LoadTableEditor<T>(string path) where T : TableRaw
        {
            string key = typeof(T).Name;

            if (!_tableCache.ContainsKey(key))
            {
                var cacheTableInstance = new CacheTable<T>();
                _tableCache[key] = cacheTableInstance;
            }

            var tableInstance = AssetDatabase.LoadAssetAtPath<BaseTableData<T>>(path);

            if (tableInstance == null)
            {
                Debug.LogError($"❌ [Editor] Failed to load table: {path}");
                return;
            }

            var cacheTable = _tableCache[key] as CacheTable<T>;

            foreach (var data in tableInstance.GetDataArray())
            {
                cacheTable.Data[data.ID] = data;
            }

            Debug.Log($"✅ [Editor] Loaded Table: {key} with {cacheTable.Data.Count} entries.");
        }
        
        
        public CacheTable<T> GetCacheTable<T>() where T : TableRaw
        {
            string key = typeof(T).Name;

            if (_tableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                    return datatable;
            }

            return null;
        }

        public string[] GetTablePath<T>() where T : TableRaw
        {
            var type = typeof(T);
            
            string jsonPath = AddressableAssetPath.CacheAssetPath[TableDataJsonKey];
            var jsonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonPath);

            if (jsonAsset == null)
            {
                Debug.LogError($"❌ Failed to load TableData.json at {jsonPath}");
                return null;
            }

            string json = jsonAsset.text;
            _tableHierarchy = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            if (_tableHierarchy.TryGetValue(type.FullName, out var output)) return output.ToArray();
            return null;
        }

        public TTable LoadOriginTableEditor<TTable>(string tableData) where TTable : ScriptableObject
        {
            var path = AddressableAssetPath.CacheAssetPath[tableData];

            var data = UnityEditor.AssetDatabase.LoadAssetAtPath<TTable>(path);
            
            if (data == null)
            {
                Debug.LogError($"Failed to load table data at path: {tableData}");
                return null;
            }

            return data;
        }

        
#endif

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

        public void Clear()
        {
            
        }
    }
}