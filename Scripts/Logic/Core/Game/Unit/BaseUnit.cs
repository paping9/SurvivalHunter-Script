using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game
{
    public class BaseUnit : MonoBehaviour, IDisposable
    {
        [SerializeField] protected Transform _root;
        [SerializeField] protected Animator _animator;
        
        public int UnitId { get; private set; }
        
        protected Dictionary<Type, IUnitComponent> _components = new();
        protected bool _isDisposed = false;
        
        public virtual void Initialize(int id)
        {
            UnitId = id;
        }
        
        public virtual void Dispose()
        {
            if (_isDisposed) return;

            foreach (var component in _components)
                component.Value.Release();

            _components.Clear();
        }


        public T Get<T>() where T : class, IUnitComponent
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null;
        }

        protected void AddComponent(IUnitComponent component)
        {
            _components.Add(component.GetType(), component);
        }

        protected void RemoveComponent(IUnitComponent component)
        {
            _components.Remove(component.GetType());
        }

        public void GameUpdate(float elapsed)
        {
            foreach (var component in _components) component.Value.GameUpdate(elapsed);
        }

        public void OnFixedUpdate(float fixedTime)
        {
            foreach (var component in _components) component.Value.FixedUpdate(fixedTime);
        }
    }
}
