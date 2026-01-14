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
        UniTask InitializeAsync();
        T[] GetTable<T>() where T : TableRaw;
        T Get<T>(int id) where T : TableRaw;
        bool TryGet<T>(int id, out T data) where T : TableRaw;
    }
}