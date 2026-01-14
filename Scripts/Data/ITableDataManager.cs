using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Table;

namespace Data
{
    public interface ICacheTable<T> where T : TableRaw
    {
        string TableName { get; }
        Dictionary<int, T> Data { get; }
    }

    public interface ITableDataManager
    {
        /// <summary>
/// Initializes the table data manager and loads all configured table caches.
/// </summary>
/// <returns>Completion of the initialization operation.</returns>
UniTask InitializeAsync();
        /// <summary>
/// Retrieve all entries for the specified table type.
/// </summary>
/// <returns>An array of table entries of type <typeparamref name="T"/>; empty if the table contains no entries.</returns>
T[] GetTable<T>() where T : TableRaw;
        /// <summary>
/// Retrieves the table entry of type T with the specified integer identifier.
/// </summary>
/// <typeparam name="T">The table row type.</typeparam>
/// <param name="id">The integer identifier of the table entry.</param>
/// <returns>The table entry of type T matching the specified id.</returns>
T Get<T>(int id) where T : TableRaw;
        /// <summary>
/// Attempts to retrieve a table entry by its integer ID.
/// </summary>
/// <param name="id">The identifier of the table entry to retrieve.</param>
/// <param name="data">When this method returns, contains the entry associated with the specified <paramref name="id"/>, if found; otherwise the default value for <typeparamref name="T"/>.</param>
/// <returns>`true` if an entry with the specified ID was found and assigned to <paramref name="data"/>, `false` otherwise.</returns>
bool TryGet<T>(int id, out T data) where T : TableRaw;
    }
}