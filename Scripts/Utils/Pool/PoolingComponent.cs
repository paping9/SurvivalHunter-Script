using System;
using UnityEngine;

namespace Utils.Pool
{
    public interface IPoolingObject
    {
        void Return();
    }

    public abstract class PoolingComponent : MonoBehaviour, IPoolingObject
    {
        public string PoolingKey { get; protected set; }
        public abstract void Return();
    }
    
    public class PoolingComponent<T> : PoolingComponent where T : Component
    {
        private IGenericPoolManager _poolManager;
        public void Setup(string poolingKey)
        {
            PoolingKey = poolingKey;
            _poolManager = SystemLocator.Get<IGenericPoolManager>();
        }
        public override void Return()
        {
            if (_poolManager != null)
                _poolManager.Release(PoolingKey, this as T);
        }
    }
}