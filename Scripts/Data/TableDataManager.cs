using System;
using System.Collections.Generic;
using System.Linq;
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
        protected readonly Dictionary<string, object> TableCache = new();
        protected Dictionary<string, List<string>> TableHierarchy = new();
        
        /// <summary>
        /// Initializes the table data manager by loading the table metadata JSON and caching all table assets.
        /// </summary>
        /// <returns>Completes when the table metadata and all referenced tables have been loaded into the manager's cache.</returns>
        public async UniTask InitializeAsync()
        {
            await LoadTableDataJson();
            await LoadAllTables();
        }
        
        /// <summary>
        /// Loads the TableData.json TextAsset from addressables and deserializes it into <see cref="TableHierarchy"/>.
        —/// </summary>
        /// <remarks>
        /// If loading fails the method logs an error and returns without modifying <see cref="TableHierarchy"/>. On success it populates <see cref="TableHierarchy"/> with the deserialized dictionary and logs the number of table groups found.
        /// </remarks>
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
            TableHierarchy = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            Debug.Log($"✅ Loaded TableData.json: {TableHierarchy.Count} table groups found.");
        }
        
        /// <summary>
        /// Load and cache every table asset listed in <see cref="TableHierarchy"/>.
        /// </summary>
        /// <returns>A UniTask that completes when all table loading operations have finished.</returns>
        protected async UniTask LoadAllTables()
        {
            var tasks = new List<UniTask>();
            foreach (var entry in TableHierarchy)
            {
                foreach (string path in entry.Value)
                {
                    tasks.Add(LoadTableAsync<TableRaw>(path));
                }
            }
            await UniTask.WhenAll(tasks);
        }
        
        /// <summary>
        /// Loads a table asset from the given addressable path and populates the cache for type T.
        /// </summary>
        /// <param name="path">Addressable key or path of the BaseTableData&lt;T&gt; asset to load.</param>
        /// <returns>`CacheTable&lt;T&gt;` containing the cached entries for the table type, or `null` if the asset failed to load.</returns>
        /// <remarks>Ensures TableCache contains a CacheTable&lt;T&gt; instance and fills it with the loaded entries keyed by each item's ID.</remarks>
        public async UniTask<CacheTable<T>> LoadTableAsync<T>(string path) where T : TableRaw
        {
            string key = typeof(T).Name;

            if (!TableCache.ContainsKey(key))
            {
                Type cacheTableType = typeof(CacheTable<>).MakeGenericType(typeof(T));
                object cacheTableInstance = Activator.CreateInstance(cacheTableType);
                TableCache[key] = cacheTableInstance;
            }

            var handle = Addressables.LoadAssetAsync<BaseTableData<T>>(path);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var tableInstance = handle.Result;
                var cacheTable = TableCache[key] as CacheTable<T>;

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

        /// <summary>
        /// Retrieves all cached entries for the table type T.
        /// </summary>
        /// <returns>An array of cached entries of type T if present; otherwise null.</returns>
        public T[] GetTable<T>() where T : TableRaw
        {
            string key = typeof(T).Name;

            if (TableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                    return datatable.Data.Values.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Retrieves a cached table entry by its identifier for the specified table type.
        /// </summary>
        /// <typeparam name="T">The table data type to retrieve.</typeparam>
        /// <param name="id">The identifier of the table entry to retrieve.</param>
        /// <returns>The cached item with the specified id, or null if not found.</returns>
        public T Get<T>(int id) where T : TableRaw
        {
            string key = typeof(T).Name;

            if (TableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                {
                    if (datatable.Data.TryGetValue(id, out var data)) return data;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Attempts to retrieve a cached table entry of type T by its ID.
        /// </summary>
        /// <param name="id">Identifier of the table entry to retrieve.</param>
        /// <param name="data">When this method returns, contains the retrieved entry if found; otherwise `null`.</param>
        /// <returns>`true` if an entry with the specified ID exists in the cache for type T and is assigned to <paramref name="data"/>, `false` otherwise.</returns>
        public bool TryGet<T>(int id, out T data) where T : TableRaw
        {
            string key = typeof(T).Name;
            if (TableCache.TryGetValue(key, out var cacheTable))
            {
                if (cacheTable is CacheTable<T> datatable)
                {
                    return datatable.Data.TryGetValue(id, out data);
                }
            }
            data = null;
            return false;
        }

        /// <summary>
        /// Intentionally does nothing; calling this method does not modify cached table data.
        /// </summary>
        /// <remarks>
        /// Reserved for API compatibility; the cache is preserved when this method is invoked.
        /// </remarks>
        public void Clear()
        {
        }
    }
}