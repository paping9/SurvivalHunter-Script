using System;
using UnityEngine;
using VContainer;

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

        [Inject]
        public void Construct(IGenericPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public void Setup(string poolingKey)
        {
            PoolingKey = poolingKey;
        }
        public override void Return()
        {
            if (_poolManager != null)
                _poolManager.Release(PoolingKey, this as T);
        }
    }
}