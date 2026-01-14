using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils.Pool
{
    public interface IGenericPoolManager
    {
        UniTask<T> Get<T>(string key, int count = 30) where T : Component;
        T GetClass<T>(string key, int count = 30) where T : class, new();
        GenericPool<T> GetOrCreate<T>(string key, IGenericPoolStrategy<T> strategy, int count = 30) where T : class;
        void Release<T>(string key, T item) where T : class;
        void Clear(string key);
        void ClearAll();
    }
}