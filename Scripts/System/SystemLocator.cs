using System.Collections.Generic;
using AssetBundle;
using Game.Map;
using Message;
using Utils.Pool;

namespace System
{
    public static partial class SystemLocator
    {
        private static readonly Dictionary<Type, object> _systems = new();

        private static bool _isInitialized = false;

        public static void Init()
        {
            if (_isInitialized) return;
            
            Register<IAddressableManager>(new AddressableManager());
            Register<IMessageBus>(new MessageBus());
            Register<IGenericPoolManager>(new GenericPoolManager());
            Register<IMapRoomCreator>(new MapRoomCreator(Get<IGenericPoolManager>()));
            
            _isInitialized = true;
        }

        public static void Register<T>(Func<T> creator)
        {
            if (Exists<T>()) return;
            
            if (creator != null)
            {
                var instance = creator.Invoke();
                Register<T>(instance);
            }
        }
        
        public static void Register<T>(T service)
        {
            var type = typeof(T);

            if (_systems.ContainsKey(type))
            {
                UnityEngine.Debug.LogWarning($"[SystemLocator] 이미 등록된 시스템입니다: {type.Name}");
                return;
            }

            _systems[type] = service;
        }

        public static T Get<T>()
        {
            var type = typeof(T);

            if (_systems.TryGetValue(type, out var service))
                return (T)service;

            UnityEngine.Debug.LogError($"[SystemLocator] 등록되지 않은 시스템 요청: {type.Name}");
            return default;
        }

        public static void Clear()
        {
            _systems.Clear();
        }

        public static bool Exists<T>() => _systems.ContainsKey(typeof(T));
    }
}